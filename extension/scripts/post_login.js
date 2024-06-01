function handleLogin(event) {
  event.preventDefault();
  console.log("Login form submitted!");

  var formData = new FormData(document.getElementById("form"));
  var jsonData = JSON.stringify(Object.fromEntries(formData.entries()));

  sendLoginRequest(jsonData);
}

function sendLoginRequest(jsonData) {
  fetch("https://cyberapi.azurewebsites.net/User/login", {
    method: "POST",
    body: jsonData,
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then(handleResponseLogin)
    .catch(handleErrorLogin);
}

async function handleResponseLogin(response) {
  if (response.status === 200) {
    const user = await response.text();
    localStorage.setItem("user", user);
    localStorage.setItem("sessionToken", response.token);
    redirectToView("views/main_view.html");
  } else if (response.status === 401) {
    displayErrorMessageLogin("Password is incorrect!");
  } else if (response.status === 404) {
    displayErrorMessageLogin("User does not exists!");
  } else {
    console.error("Response status is not 200 OK");
    throw new Error("Response status is not 200 OK");
  }
}

function handleErrorLogin(error) {
  console.error("Request failed", error);
}

function displayErrorMessageLogin(message) {
  document.getElementById("response").innerHTML = message;
}

document.getElementById("loginBtn").addEventListener("click", handleLogin);
