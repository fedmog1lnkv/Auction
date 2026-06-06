document.addEventListener("DOMContentLoaded", () => {
  initSearchForm();
  initFilters();
  initBidsTabs();
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