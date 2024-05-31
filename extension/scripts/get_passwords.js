function handleGetPassword(event) {
  //event.preventDefault();
  console.log("Get passwords form submitted!");

  var json = JSON.stringify({
    username: sessionStorage.getItem("user")
  });

  console.log(json);
  sendGetPasswordRequest(json);
}

function sendGetPasswordRequest(jsonData) {
  fetch("https://cyberapi.azurewebsites.net/User/password/get", {
    method: "POST",
    body: jsonData,
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then(handleResponseGetPassword)
    .catch(handleErrorGetPassword);
}

async function handleResponseGetPassword(response) {
  //console.log(response);
  if (response.status === 200) {
    const data = await response.json();
    let hashtable = new Map(Object.entries(data));
    console.log(hashtable);

    for (let [siteAddress, plainPassword] of hashtable) {
      console.log(siteAddress + " = " + plainPassword);
      document.getElementById("content").innerHTML +=
        "<p>" + siteAddress + " = " + plainPassword + "</p>";
    }

    // response.json().then((data) => {
    //   redirectToView("views/main_view.html").then(() => {
    //     const content = document.getElementById("content");

    //     data.forEach((item) => {
    //       const siteAddressElement = document.createElement("p");
    //       siteAddressElement.textContent = item.siteAddress;
    //       content.appendChild(siteAddressElement);

    //       const usernameElement = document.createElement("p");
    //       usernameElement.textContent = item.username;
    //       content.appendChild(usernameElement);

    //       const passwordElement = document.createElement("p");
    //       passwordElement.textContent = item.password;
    //       content.appendChild(passwordElement);
    //     });
    //   });
    // });
  } else {
    console.error("Response status is not 200 OK");
    throw new Error("Response status is not 200 OK");
  }
}

function handleErrorGetPassword(error) {
  console.error("Request failed", error);
}

function displayErrorMessageGetPassword(message) {
  document.getElementById("response").innerHTML = message;
}
