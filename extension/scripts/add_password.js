function handleAddPassword(event) {
  event.preventDefault();
  console.log("Form submitted!");

  var formData = new FormData(document.getElementById("form"));
  var jsonData = JSON.stringify(Object.fromEntries(formData.entries()));

  sendAddPasswordRequest(jsonData);
}

function sendAddPasswordRequest(jsonData) {
  fetch("https://cyberapi.azurewebsites.net/User/password/Save", {
    method: "POST",
    body: jsonData,
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then(handleResponseAddPassword)
    .catch(handleErrorAddPassword);
}

async function handleResponseAddPassword(response) {
  console.log(response);
  if (response.status === 201) {
    const data = response.json();
    redirectToView("views/main_view.html");
  } else if (response.status === 500) {
    displayErrorMessage("ERROR!");
  } else {
    console.error("Response status is not 200 OK");
    throw new Error("Response status is not 200 OK");
  }
}

function handleErrorAddPassword(error) {
  console.error("Request failed", error);
}

function displayErrorMessageAddPassword(message) {
  document.getElementById("response").innerHTML = message;
}

document
  .getElementById("submitBtn")
  .addEventListener("click", handleAddPassword);

document.getElementById("backBtn").addEventListener("click", () => {
  redirectToView("views/main_view.html");
});
