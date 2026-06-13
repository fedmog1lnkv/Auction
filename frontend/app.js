var API_URL = "http://186.246.31.83:8080";
var notificationsData = [];

document.addEventListener("DOMContentLoaded", function () {
  initSearchForm();
  initFilters();
  initBidsTabs();

  login();
  registration();
  profilePage();
  logout();
  profileLink();
  tabsLoginPage();

  loadProfileFromBackend();
  loadProfileLots();
  settingsPage();
  notificationsPage();
});

function initSearchForm() {
  const searchForm = document.getElementById("searchForm");

  if (!searchForm) {
    return;
  }

  searchForm.addEventListener("submit", (event) => {
    event.preventDefault();
  });
}

function initFilters() {
  const resetButton = document.getElementById("resetFilters");

  if (!resetButton) {
    return;
  }

  resetButton.addEventListener("click", () => {
    const statusFilter = document.getElementById("statusFilter");
    const priceFrom = document.getElementById("priceFrom");
    const priceTo = document.getElementById("priceTo");
    const searchInput = document.getElementById("searchInput");

    if (statusFilter) {
      statusFilter.value = "";
    }

    if (priceFrom) {
      priceFrom.value = "";
    }

    if (priceTo) {
      priceTo.value = "";
    }

    if (searchInput) {
      searchInput.value = "";
    }
  });
}

function initBidsTabs() {
  const tabs = document.querySelectorAll(".tab");
  const tableBody = document.getElementById("bidsTableBody");

  if (!tabs.length || !tableBody) {
    return;
  }

  const emptyMessages = {
    active: "Активных ставок пока нет",
    won: "Выигранных лотов пока нет",
    watching: "Отслеживаемых лотов пока нет"
  };

  tabs.forEach((tab) => {
    tab.addEventListener("click", () => {
      tabs.forEach((item) => item.classList.remove("active"));
      tab.classList.add("active");

      const tabName = tab.dataset.tab;
      const message = emptyMessages[tabName] || "Данные пока недоступны";

      tableBody.innerHTML = `
        <tr class="empty-row">
          <td colspan="6">${message}</td>
        </tr>
      `;
    });
  });
}

function login() {
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
          throw new Error("Ошибка входа");
        }
        return response.json();
      })
      .then(function (data) {
        saveUser(data);
        alert("Вы вошли в аккаунт");
        window.location.href = "profile.html";
      })
      .catch(function () {
        alert("Не получилось войти. Введите корректный email и пароль");
      });
  });
}

function registration() {
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
        firstName: firstName,
        lastName: lastName,
        email: email,
        password: password
      })
    })
      .then(function (response) {
        if (response.ok == false) {
          throw new Error("Ошибка регистрации");
        }
        return response.json();
      })
      .then(function (data) {
        saveUser(data);
        alert("Аккаунт создан");
        window.location.href = "profile.html";
      })
      .catch(function () {
        alert("Не удалосью зарегистрироваться, попробуйте позже");
      });
  });
}

function saveUser(data) {
  var token = getValue(data, "accessToken", "AccessToken");
  var refreshToken = getValue(data, "refreshToken", "RefreshToken");
  var userId = getValue(data, "userId", "UserId");
  var email = getValue(data, "email", "Email");
  var firstName = getValue(data, "firstName", "FirstName");
  var lastName = getValue(data, "lastName", "LastName");

  if (token != null) {
    localStorage.setItem("token", token);
  }

  if (refreshToken != null) {
    localStorage.setItem("refreshToken", refreshToken);
  }

  saveProfileData(userId, email, firstName, lastName);
}

function saveProfileData(userId, email, firstName, lastName) {
  if (userId != null) {
    localStorage.setItem("userId", userId);
  }

  if (email != null) {
    localStorage.setItem("email", email);
  }

  if (firstName != null) {
    localStorage.setItem("firstName", firstName);
  }

  if (lastName != null) {
    localStorage.setItem("lastName", lastName);
  }
}

function profilePage() {
  var page = document.body.getAttribute("data-page");

  if (page == "profile" || page == "settings" || page == "notifications") {
    if (localStorage.getItem("token") == null) {
      window.location.href = "loginform.html";
      return;
    }
  }

  fillUserBlocks();
}

function fillUserBlocks() {
  var nameBlock = document.querySelector("[data-user-name]");
  var emailBlock = document.querySelector("[data-user-email]");
  var initialsBlock = document.querySelector("[data-user-initials]");

  var firstName = localStorage.getItem("firstName") || "";
  var lastName = localStorage.getItem("lastName") || "";
  var email = localStorage.getItem("email") || "";

  if (nameBlock != null) {
    nameBlock.textContent = firstName + " " + lastName;
  }

  if (emailBlock != null) {
    emailBlock.textContent = email;
  }

  if (initialsBlock != null) {
    initialsBlock.textContent = firstName.charAt(0) + lastName.charAt(0);
  }
}

function logout() {
  var button = document.getElementById("logoutBtn");

  if (button == null) {
    return;
  }

  button.addEventListener("click", function () {
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("userId");
    localStorage.removeItem("email");
    localStorage.removeItem("firstName");
    localStorage.removeItem("lastName");

    window.location.href = "loginform.html";
  });
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

function tabsLoginPage() {
  var loginForm = document.getElementById("loginForm");
  var regForm = document.getElementById("regForm");
  var buttons = document.querySelectorAll(".tabBtn");

  if (loginForm == null || regForm == null) {
    return;
  }

  loginForm.style.display = "block";
  regForm.style.display = "none";

  for (var i = 0; i < buttons.length; i++) {
    buttons[i].addEventListener("click", function () {
      for (var j = 0; j < buttons.length; j++) {
        buttons[j].classList.remove("active");
      }

      this.classList.add("active");

      if (this.getAttribute("data-tab") == "loginForm") {
        loginForm.style.display = "block";
        regForm.style.display = "none";
      } else {
        loginForm.style.display = "none";
        regForm.style.display = "block";
      }
    });
  }
}

function loadProfileFromBackend() {
  var page = document.body.getAttribute("data-page");

  if (page != "profile" && page != "settings") {
    return;
  }

  var token = localStorage.getItem("token");

  if (token == null) {
    return;
  }

  fetch(API_URL + "/api/profile", {
    method: "GET",
    headers: {
      "Authorization": "Bearer " + token
    }
  })
    .then(function (response) {
      if (response.ok == false) {
        return null;
      }
      return response.json();
    })
    .then(function (data) {
      if (data == null) {
        return;
      }

      saveProfileData(
        getValue(data, "userId", "UserId"),
        getValue(data, "email", "Email"),
        getValue(data, "firstName", "FirstName"),
        getValue(data, "lastName", "LastName")
      );

      fillUserBlocks();
      fillSettingsForm();
    })
    .catch(function () {
      fillSettingsForm();
    });
}

function loadProfileLots() {
  var page = document.body.getAttribute("data-page");
  var block = document.getElementById("bids");
  var userId = localStorage.getItem("userId");

  if (page != "profile" || block == null || userId == null) {
    return;
  }

  block.innerHTML = "<h2>Мои лоты</h2><p class='empty-message'>Загрузка лотов...</p>";

  fetch(API_URL + "/lots?seller_id=" + encodeURIComponent(userId) + "&page=1&limit=5", {
    method: "GET",
    headers: getAuthHeaders()
  })
    .then(function (response) {
      if (response.ok == false) {
        throw new Error("Ошибка загрузки лотов");
      }
      return response.json();
    })
    .then(function (data) {
      var lots = getItems(data);

      if (lots.length == 0) {
        block.innerHTML = "<h2>Мои лоты</h2><p class='empty-message'>У вас пока нет созданных лотов</p>";
        return;
      }

      var html = "<h2>Мои лоты</h2><table class='bids-table'><tbody>";

      for (var i = 0; i < lots.length; i++) {
        var lot = lots[i];
        html += "<tr>";
        html += "<td>" + escapeHtml(getValue(lot, "title", "Title")) + "</td>";
        html += "<td>" + statusToText(getValue(lot, "status", "Status")) + "</td>";
        html += "<td>" + getValue(lot, "currentPrice", "CurrentPrice") + " ₽</td>";
        html += "<td>" + dateToText(getValue(lot, "endsAt", "EndsAt")) + "</td>";
        html += "</tr>";
      }

      html += "</tbody></table>";
      block.innerHTML = html;
    })
    .catch(function () {
      block.innerHTML = "<h2>Мои лоты</h2><p class='empty-message'>Не получилось загрузить лоты с сервера</p>";
    });
}

function settingsPage() {
  var page = document.body.getAttribute("data-page");

  if (page != "settings") {
    return;
  }

  fillSettingsForm();

  var form = document.getElementById("settingsForm");

  if (form != null) {
    form.addEventListener("submit", function (event) {
      event.preventDefault();

      var firstName = form.elements["firstName"].value;
      var lastName = form.elements["lastName"].value;

      localStorage.setItem("phone", form.elements["phone"].value);
      localStorage.setItem("about", form.elements["about"].value);
      localStorage.setItem("theme", form.elements["theme"].value);
      localStorage.setItem("rows", form.elements["rows"].value);

      fetch(API_URL + "/api/profile", {
        method: "PATCH",
        headers: getAuthHeaders(),
        body: JSON.stringify({
          firstName: firstName,
          lastName: lastName
        })
      })
        .then(function (response) {
          if (response.ok == false) {
            throw new Error("Ошибка сохранения");
          }
          return response.json();
        })
        .then(function (data) {
          saveProfileData(
            getValue(data, "userId", "UserId"),
            getValue(data, "email", "Email"),
            getValue(data, "firstName", "FirstName"),
            getValue(data, "lastName", "LastName")
          );

          alert("Настройки сохранены");
        })
        .catch(function () {
          alert("Не получилось сохранить профиль на сервере");
        });
    });
  }

  var clearButton = document.getElementById("clearLocal");

  if (clearButton != null) {
    clearButton.addEventListener("click", function () {
      localStorage.removeItem("phone");
      localStorage.removeItem("about");
      localStorage.removeItem("theme");
      localStorage.removeItem("rows");
      fillSettingsForm();
      alert("Локальные настройки очищены");
    });
  }
}

function fillSettingsForm() {
  var form = document.getElementById("settingsForm");

  if (form == null) {
    return;
  }

  form.elements["firstName"].value = localStorage.getItem("firstName") || "";
  form.elements["lastName"].value = localStorage.getItem("lastName") || "";
  form.elements["email"].value = localStorage.getItem("email") || "";
  form.elements["email"].readOnly = true;
  form.elements["phone"].value = localStorage.getItem("phone") || "";
  form.elements["about"].value = localStorage.getItem("about") || "";
  form.elements["theme"].value = localStorage.getItem("theme") || "light";
  form.elements["rows"].value = localStorage.getItem("rows") || "10";
}

function notificationsPage() {
  var page = document.body.getAttribute("data-page");
  var list = document.getElementById("notiList");

  if (page != "notifications" || list == null) {
    return;
  }

  list.innerHTML = "<p class='empty-message'>Загрузка уведомлений...</p>";

  fetch(API_URL + "/api/notifications", {
    method: "GET",
    headers: getAuthHeaders()
  })
    .then(function (response) {
      if (response.ok == false) {
        throw new Error("Ошибка уведомления");
      }
      return response.json();
    })
    .then(function (data) {
      notificationsData = data;
      showNotifications("all");
    })
    .catch(function () {
      list.innerHTML = "<p class='empty-message'>Не получилось загрузить уведомления с сервера</p>";
    });

  var filterButtons = document.querySelectorAll(".pill");

  for (var i = 0; i < filterButtons.length; i++) {
    filterButtons[i].addEventListener("click", function () {
      for (var j = 0; j < filterButtons.length; j++) {
        filterButtons[j].classList.remove("active");
      }

      this.classList.add("active");
      showNotifications(this.getAttribute("data-filter"));
    });
  }

  var allRead = document.getElementById("allRead");

  if (allRead != null) {
    allRead.addEventListener("click", function () {
      for (var k = 0; k < notificationsData.length; k++) {
        notificationsData[k].isRead = true;
      }
      showNotifications("all");
    });
  }

  var clearRead = document.getElementById("clearRead");

  if (clearRead != null) {
    clearRead.addEventListener("click", function () {
      notificationsData = [];
      showNotifications("all");
    });
  }
}

function showNotifications(filter) {
  var list = document.getElementById("notiList");

  if (list == null) {
    return;
  }

  var html = "";
  var count = 0;

  for (var i = 0; i < notificationsData.length; i++) {
    var note = notificationsData[i];
    var type = getValue(note, "type", "Type");
    var isRead = getValue(note, "isRead", "IsRead");

    if (filter == "unread" && isRead == true) {
      continue;
    }

    if (filter != "all" && filter != "unread" && filter != type) {
      continue;
    }

    count++;
    html += "<div class='table-card' style='margin-bottom:12px'>";
    html += "<b>" + escapeHtml(getValue(note, "title", "Title")) + "</b>";
    html += "<p>" + escapeHtml(getValue(note, "text", "Text")) + "</p>";
    html += "<small>" + dateToText(getValue(note, "createdAt", "CreatedAt")) + "</small>";
    html += "</div>";
  }

  if (count == 0) {
    html = "<p class='empty-message'>Уведомлений пока нет</p>";
  }

  list.innerHTML = html;
}

function requestWithToken(url, method, data) {
  var options = {
    method: method,
    headers: getAuthHeaders()
  };

  if (data != null) {
    options.body = JSON.stringify(data);
  }

  return fetch(API_URL + url, options);
}

function getAuthHeaders() {
  var token = localStorage.getItem("token");

  return {
    "Content-Type": "application/json",
    "Authorization": "Bearer " + token
  };
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
    return null;
  }

  if (obj[smallName] != null) {
    return obj[smallName];
  }

  if (obj[bigName] != null) {
    return obj[bigName];
  }

  return null;
}

function statusToText(status) {
  if (status == "DRAFT") {
    return "Черновик";
  }

  if (status == "ACTIVE") {
    return "Активный";
  }

  if (status == "FINISHED") {
    return "Завершён";
  }

  if (status == "CANCELLED") {
    return "Отменён";
  }

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
  if (text == null) {
    return "";
  }

  return String(text)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/\"/g, "&quot;")
    .replace(/'/g, "&#039;");
}
