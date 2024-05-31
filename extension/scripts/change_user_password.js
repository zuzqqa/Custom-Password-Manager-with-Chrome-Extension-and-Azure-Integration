function handleChangePassword(event) {
  event.preventDefault();
  console.log("Change password submitted!");

  var formData = new FormData(document.getElementById("form"));
  var jsonData = JSON.stringify(Object.fromEntries(formData.entries()));

  sendChangePasswordRequest(jsonData);
}

function sendChangePasswordRequest(jsonData) {
  fetch("https://cyberapi.azurewebsites.net/User/update", {
    method: "PUT",
    body: jsonData,
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then(handleResponseChangePassword)
    .catch(handleErrorChangePassword);
}

async function handleResponseChangePassword(response) {
  console.log(response);
  if (response.status === 200) {
    const data = response.json();
    displayErrorMessageChangePassword(
      "Password has been changed successfully!"
    );
    redirectToView("views/login_view.html");
  } else if (response.status === 404) {
    displayErrorMessageChangePassword("User does not exist!");
  } else if (response.status === 500) {
    displayErrorMessageChangePassword("Error!");
  } else {
    console.error("Response status is not 200 OK");
    throw new Error("Response status is not 200 OK");
  }
}

function handleErrorChangePassword(error) {
  console.error("Request failed", error);
}

function displayErrorMessageChangePassword(message) {
  document.getElementById("response").innerHTML = message;
}

document
  .getElementById("submitBtn")
  .addEventListener("click", handleChangePassword);

document
  .getElementById("backBtn")
  .addEventListener("click", () => redirectToView("views/login_view.html"));
