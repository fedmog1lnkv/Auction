var API_URL = "http://186.246.31.83:8080";


document.addEventListener("DOMContentLoaded", function () {
  initSearchForm();
  initFilters();
  initBidsTabs();
  createLotForm();
  loginTabs();
  loginForm();
  registerForm();
  checkPrivatePages();
  fillUserInfo();
  loadMyLots();
  settingsPage();
  logoutButtons();
  profileLink();
  loadLots();
});

function initSearchForm() {
  var searchForm = document.getElementById("searchForm");

  if (searchForm == null) {
    return;
  }

  searchForm.addEventListener("submit", function (event) {
    event.preventDefault();
  });
}

function initFilters() {
  var resetButton = document.getElementById("resetFilters");

  if (resetButton == null) {
    return;
  }

  resetButton.addEventListener("click", function () {
    var statusFilter = document.getElementById("statusFilter");
    var priceFrom = document.getElementById("priceFrom");
    var priceTo = document.getElementById("priceTo");
    var searchInput = document.getElementById("searchInput");

    if (statusFilter != null) statusFilter.value = "";
    if (priceFrom != null) priceFrom.value = "";
    if (priceTo != null) priceTo.value = "";
    if (searchInput != null) searchInput.value = "";
  });
}

function initBidsTabs() {
  var tabs = document.querySelectorAll(".tab");
  var tableBody = document.getElementById("bidsTableBody");

  if (tabs.length == 0 || tableBody == null) {
    return;
  }

  for (var i = 0; i < tabs.length; i++) {
    tabs[i].addEventListener("click", function () {
      for (var j = 0; j < tabs.length; j++) {
        tabs[j].classList.remove("active");
      }

      this.classList.add("active");

      tableBody.innerHTML = "<tr class='empty-row'><td colspan='6'>Ставок нет</td></tr>";
    });
  }
}

function loginTabs() {
  var login = document.getElementById("loginForm");
  var reg = document.getElementById("regForm");
  var buttons = document.querySelectorAll(".tabBtn");

  if (login == null || reg == null) {
    return;
  }

  login.style.display = "block";
  reg.style.display = "none";

  for (var i = 0; i < buttons.length; i++) {
    buttons[i].addEventListener("click", function () {
      for (var j = 0; j < buttons.length; j++) {
        buttons[j].classList.remove("active");
      }

      this.classList.add("active");

      if (this.getAttribute("data-tab") == "loginForm") {
        login.style.display = "block";
        reg.style.display = "none";
      } else {
        login.style.display = "none";
        reg.style.display = "block";
      }
    });
  }
}

function loginForm() {
  var form = document.getElementById("loginForm");

  if (form == null) {
    return;
  }

  form.addEventListener("submit", function (event) {
    event.preventDefault();

    var email = document.getElementById("loginEmail").value;
    var password = document.getElementById("loginPassword").value;

    if (email == "" || password == "") {
      alert("Введите email и пароль");
      return;
    }

    fetch(API_URL + "/api/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        email: email,
        password: password
      })
    })
      .then(function (response) {
        if (response.ok == false) {
          throw new Error();
        }
        return response.json();
      })
      .then(function (data) {
        saveUser(data);
        window.location.href = "profile.html";
      })
      .catch(function () {
        alert("Не получилось войти. Проверьте email и пароль");
      });
  });
}

function registerForm() {
  var form = document.getElementById("regForm");

  if (form == null) {
    return;
  }

  form.addEventListener("submit", function (event) {
    event.preventDefault();

    var firstName = document.getElementById("regFirstName").value;
    var lastName = document.getElementById("regLastName").value;
    var email = document.getElementById("regEmail").value;
    var password = document.getElementById("regPassword").value;

    if (firstName == "" || lastName == "" || email == "" || password == "") {
      alert("Заполните все поля");
      return;
    }

    fetch(API_URL + "/api/auth/register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        email: email,
        firstName: firstName,
        lastName: lastName,
        password: password
      })
    })
      .then(function (response) {
        if (response.ok == false) {
          throw new Error();
        }
        return response.json();
      })
      .then(function (data) {
        saveUser(data);
        window.location.href = "profile.html";
      })
      .catch(function () {
        alert("Не получилось зарегистрироваться");
      });
  });
}

function saveUser(data) {
  localStorage.setItem("token", getValue(data, "accessToken", "AccessToken"));
  localStorage.setItem("refreshToken", getValue(data, "refreshToken", "RefreshToken"));
  localStorage.setItem("userId", getValue(data, "userId", "UserId"));
  localStorage.setItem("email", getValue(data, "email", "Email"));
  localStorage.setItem("firstName", getValue(data, "firstName", "FirstName"));
  localStorage.setItem("lastName", getValue(data, "lastName", "LastName"));
}

function checkPrivatePages() {
  var page = document.body.getAttribute("data-page");

  if (page == "profile" || page == "settings" || page == "notifications") {
    if (localStorage.getItem("token") == null) {
      window.location.href = "loginform.html";
    }
  }
}

function fillUserInfo() {
  var firstName = localStorage.getItem("firstName") || "";
  var lastName = localStorage.getItem("lastName") || "";
  var email = localStorage.getItem("email") || "";
  var fullName = (firstName + " " + lastName).trim();

  var nameBlocks = document.querySelectorAll("[data-user-name]");
  var emailBlocks = document.querySelectorAll("[data-user-email]");
  var initialsBlocks = document.querySelectorAll("[data-user-initials]");

  for (var i = 0; i < nameBlocks.length; i++) {
    nameBlocks[i].textContent = fullName || "Пользователь";
  }

  for (var j = 0; j < emailBlocks.length; j++) {
    emailBlocks[j].textContent = email;
  }

  for (var k = 0; k < initialsBlocks.length; k++) {
    var first = firstName.charAt(0);
    var second = lastName.charAt(0);
    initialsBlocks[k].textContent = (first + second).toUpperCase() || "П";
  }
}

function loadMyLots() {
  var block = document.getElementById("myLotsBlock");
  var userId = localStorage.getItem("userId");

  if (block == null) {
    return;
  }

  if (userId == null || userId == "") {
    block.innerHTML = "<p class='empty-message'>Не найден id пользователя</p>";
    return;
  }

  block.innerHTML = "<p class='empty-message'>Загрузка лотов...</p>";

  fetch(API_URL + "/lots?seller_id=" + encodeURIComponent(userId) + "&page=1&limit=10")
    .then(function (response) {
      if (response.ok == false) {
        throw new Error();
      }
      return response.json();
    })
    .then(function (data) {
      var lots = getItems(data);

      if (lots.length == 0) {
        block.innerHTML = "<p class='empty-message'>Нет созданных лотов</p>";
        return;
      }

      var html = "<div class='table-card'><table class='bids-table'><thead><tr><th>Лот</th><th>Статус</th><th>Цена</th><th>Дата окончания</th></tr></thead><tbody>";

      for (var i = 0; i < lots.length; i++) {
        html += "<tr>";
        html += "<td>" + escapeHtml(getValue(lots[i], "title", "Title")) + "</td>";
        html += "<td>" + statusToText(getValue(lots[i], "status", "Status")) + "</td>";
        html += "<td>" + getValue(lots[i], "currentPrice", "CurrentPrice") + " ₽</td>";
        html += "<td>" + dateToText(getValue(lots[i], "endsAt", "EndsAt")) + "</td>";
        html += "</tr>";
      }

      html += "</tbody></table></div>";
      block.innerHTML = html;
    })
    .catch(function () {
      block.innerHTML = "<p class='empty-message'>Не получилось загрузить лоты</p>";
    });
}

function settingsPage() {
  var form = document.getElementById("settingsInfoForm");

  if (form == null) {
    return;
  }

  form.elements["firstName"].value = localStorage.getItem("firstName") || "";
  form.elements["lastName"].value = localStorage.getItem("lastName") || "";
  form.elements["email"].value = localStorage.getItem("email") || "";
}

function logoutButtons() {
  var buttons = document.querySelectorAll("[data-logout]");

  for (var i = 0; i < buttons.length; i++) {
    buttons[i].addEventListener("click", function () {
      clearUserData();
      window.location.href = "loginform.html";
    });
  }
}

function clearUserData() {
  localStorage.removeItem("token");
  localStorage.removeItem("refreshToken");
  localStorage.removeItem("userId");
  localStorage.removeItem("email");
  localStorage.removeItem("firstName");
  localStorage.removeItem("lastName");
}

function profileLink() {
  var links = document.querySelectorAll(".profile-link");

  for (var i = 0; i < links.length; i++) {
    if (localStorage.getItem("token") == null) {
      links[i].href = "loginform.html";
    } else {
      links[i].href = "profile.html";
    }
  }
}

function getItems(data) {
  var items = getValue(data, "items", "Items");

  if (items == null) {
    return [];
  }

  return items;
}

function getValue(obj, smallName, bigName) {
  if (obj == null) {
    return "";
  }

  if (obj[smallName] != null) {
    return obj[smallName];
  }

  if (obj[bigName] != null) {
    return obj[bigName];
  }

  return "";
}

function statusToText(status) {
  if (status == "DRAFT") return "Черновик";
  if (status == "ACTIVE") return "Активный";
  if (status == "FINISHED") return "Завершён";
  if (status == "CANCELLED") return "Отменён";
  return status || "Неизвестно";
}

function dateToText(value) {
  if (value == null || value == "") {
    return "";
  }

  var date = new Date(value);

  if (isNaN(date.getTime())) {
    return value;
  }

  return date.toLocaleString("ru-RU");
}

function escapeHtml(text) {
  return String(text || "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/\"/g, "&quot;")
    .replace(/'/g, "&#039;");
}
function createLotForm() {
  var form = document.getElementById("createLotForm");

  if (form == null) {
    return;
  }

  form.addEventListener("submit", function (event) {
    event.preventDefault();

    var message = document.getElementById("createLotMessage");
    var token = localStorage.getItem("token");

    if (token == null || token == "") {
      message.textContent = "Сначала войдите в аккаунт.";
      return;
    }

    var startsAtValue = document.getElementById("startsAtInput").value;
    var endsAtValue = document.getElementById("endsAtInput").value;

    var lotData = {
      title: document.getElementById("lotTitleInput").value.trim(),
      description: document.getElementById("lotDescriptionInput").value.trim(),
      startingPrice: Number(document.getElementById("startingPriceInput").value),
      minBidStep: Number(document.getElementById("minBidStepInput").value),
      startsAt: new Date(startsAtValue).toISOString(),
      endsAt: new Date(endsAtValue).toISOString()
    };

    message.textContent = "Создаём лот...";

    fetch(API_URL + "/lots", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer " + token
      },
      body: JSON.stringify(lotData)
    })
      .then(function (response) {
        return response.json().then(function (data) {
          return {
            ok: response.ok,
            data: data
          };
        });
      })
      .then(function (result) {
        if (result.ok == false) {
          throw new Error(result.data.message || "Не удалось создать лот");
        }

        message.textContent = "Лот создан.";

        var lotId = getValue(result.data, "id", "Id");

        if (lotId != "") {
          window.location.href = "lot.html?id=" + encodeURIComponent(lotId);
        } else {
          window.location.href = "index.html";
        }
      })
      .catch(function (error) {
        message.textContent = error.message;
      });
  });
}
function loadLots() {
  var lotsList = document.getElementById("lotsList");

  if (lotsList == null) {
    return;
  }

  lotsList.innerHTML = "<p class='empty-message'>Загрузка лотов...</p>";

  fetch(API_URL + "/lots?page=1&limit=20")
    .then(function (response) {
      if (response.ok == false) {
        throw new Error("Не удалось загрузить лоты");
      }

      return response.json();
    })
    .then(function (data) {
      var lots = getItems(data);
      renderLots(lots);
    })
    .catch(function () {
      lotsList.innerHTML = "<p class='empty-message'>Не удалось загрузить лоты</p>";
    });
}

function renderLots(lots) {
  var lotsList = document.getElementById("lotsList");

  if (lotsList == null) {
    return;
  }

  if (lots.length == 0) {
    lotsList.innerHTML = "<p class='empty-message'>Лотов пока нет</p>";
    return;
  }

  var html = "";

  for (var i = 0; i < lots.length; i++) {
    var id = getValue(lots[i], "id", "Id");
    var title = getValue(lots[i], "title", "Title");
    var description = getValue(lots[i], "description", "Description");
    var currentPrice = getValue(lots[i], "currentPrice", "CurrentPrice");
    var status = getValue(lots[i], "status", "Status");
    var endsAt = getValue(lots[i], "endsAt", "EndsAt");

    html += "<article class='lot-card'>";
    html += "<div class='lot-card-content'>";
    html += "<h3>" + escapeHtml(title) + "</h3>";

    html += "<p class='lot-card-description'>";
    html += escapeHtml(description || "Описание не добавлено");
    html += "</p>";

    html += "<div class='lot-card-row'>";
    html += "<span>Текущая цена</span>";
    html += "<strong>" + priceToText(currentPrice) + "</strong>";
    html += "</div>";

    html += "<div class='lot-card-row'>";
    html += "<span>Статус</span>";
    html += "<strong>" + statusToText(status) + "</strong>";
    html += "</div>";

    html += "<div class='lot-card-row'>";
    html += "<span>Окончание</span>";
    html += "<strong>" + dateToText(endsAt) + "</strong>";
    html += "</div>";

    html += "<a class='primary-button lot-card-link' href='lot.html?id=" + encodeURIComponent(id) + "'>";
    html += "Открыть лот";
    html += "</a>";

    html += "</div>";
    html += "</article>";
  }

  lotsList.innerHTML = html;
}

function priceToText(value) {
  var number = Number(value || 0);

  return number.toLocaleString("ru-RU") + " ₽";
}