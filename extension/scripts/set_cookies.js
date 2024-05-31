function setCookie(name, value, seconds) {
  var expires = "";
  if (seconds) {
    var date = new Date();
    date.setTime(date.getTime() + seconds * 1000);
    expires = "; expires=" + date.toUTCString();
  }
  document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

document
  .getElementById("changePasswordBtn")
  .addEventListener("click", function () {
    var username = document.getElementById("name").value;
    setCookie("username", username, 16 * 60);
    redirectToView("views/change_password_view.html"); 
  });
