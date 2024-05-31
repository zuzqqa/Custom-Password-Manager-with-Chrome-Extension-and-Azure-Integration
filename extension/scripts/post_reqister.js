function handleRegistration(event) {
  event.preventDefault();
  console.log("Registration form submitted!");

  var formData = new FormData(document.getElementById("form"));
  var jsonData = JSON.stringify(Object.fromEntries(formData.entries()));

  sendRegistrationRequest(jsonData);
}

function sendRegistrationRequest(jsonData) {
  fetch("https://cyberapi.azurewebsites.net/User/create", {
    method: "POST",
    body: jsonData,
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then(handleResponse)
    .catch(handleError);
}

function handleResponse(response) {
  if (response.ok) {
    response.json().then((data) => {
      localStorage.setItem("sessionToken", data.token);
      displayErrorMessage("Registration successful!");
      sleep(2000).then(() => {
        displayErrorMessage("Redirecting to the main view!");
      });
      redirectToView("views/main_view.html");
    });
  } else if (response.status === 400) {
    displayErrorMessage("Exception was thrown!");
  } else if (response.status === 409) {
    displayErrorMessage("Username is already taken!");
  } else {
    console.error("Response status is not 200 OK");
    throw new Error("Response status is not 200 OK");
  }
}

function handleErrorRegister(error) {
  console.error("Request failed", error);
}

function displayErrorMessageRegister(message) {
  document.getElementById("response").innerHTML = message;
}

document
  .getElementById("registerBtn")
  .addEventListener("click", handleRegistration);

window.onload = function () {
  var sessionToken = localStorage.getItem("sessionToken");
  if (sessionToken) {
    redirectToView("views/main_view.html");
  }
};
