var API_URL = "http://186.246.31.83:8080";

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
        alert("Не получилось войти. Проверьте email и пароль");
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
        alert("Не получилось зарегистрироваться");
      });
  });
}

function saveUser(data) {
  var token = data.accessToken;
  var refreshToken = data.refreshToken;

  // На всякий случай, если сервер отдаст поля с большой буквы
  if (token == null) {
    token = data.AccessToken;
  }

  if (refreshToken == null) {
    refreshToken = data.RefreshToken;
  }

  localStorage.setItem("token", token);
  localStorage.setItem("refreshToken", refreshToken);
  localStorage.setItem("email", data.email || data.Email);
  localStorage.setItem("firstName", data.firstName || data.FirstName);
  localStorage.setItem("lastName", data.lastName || data.LastName);
}

function profilePage() {
  var page = document.body.getAttribute("data-page");

  if (page == "profile" || page == "settings") {
    if (localStorage.getItem("token") == null) {
      window.location.href = "loginform.html";
      return;
    }
  }

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

function requestWithToken(url, method, data) {
  var token = localStorage.getItem("token");

  return fetch(API_URL + url, {
    method: method,
    headers: {
      "Content-Type": "application/json",
      "Authorization": "Bearer " + token
    },
    body: JSON.stringify(data)
  });
}
