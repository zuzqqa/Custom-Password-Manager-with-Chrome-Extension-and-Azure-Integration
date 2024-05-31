document.getElementById("logoutBtn").addEventListener("click", function () {
  localStorage.removeItem("sessionToken");
  localStorage.removeItem("username");
  redirectToView("views/login_view.html");
});

document.getElementById("addBtn").addEventListener("click", function () {
  redirectToView("views/add_password_view.html");
});

document
  .getElementById("submitBtn")
  .addEventListener("click", (event) => handleGetPassword(event));

document.addEventListener("DOMContentLoaded", function () {
  console.log("DOM fully loaded and parsed");
});
