function handleGetPassword(event) {
  console.log("Get passwords form submitted!");

  var json = JSON.stringify({
    username: localStorage.getItem("user")
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
  if (response.status === 200) {
    const data = await response.json();
    let hashtable = new Map(Object.entries(data));

    for (let [siteAddress, plainPassword] of hashtable) {
      let contentDiv = document.getElementById("content");
      
      let existingElements = Array.from(contentDiv.getElementsByTagName('p'));
      if (existingElements.some(p => p.textContent.includes(siteAddress))) {
          continue;
      }

      let p = document.createElement('p');
      let span = document.createElement('span');
      let button = document.createElement('button');
      let passwordInput = document.createElement('input');
  
      span.textContent = siteAddress;
      span.style.fontSize = '16px'; 
      passwordInput.value = plainPassword;
      passwordInput.type = 'password';
      passwordInput.style.borderRadius = '5px';
      passwordInput.style.backgroundColor = 'white';
      passwordInput.style.color = 'black';
      passwordInput.style.marginTop = '10px';
      passwordInput.style.marginBottom = '10px';passwordInput.style.paddingLeft = '5px';
      passwordInput.style.paddingRight = '5px';

      button.textContent = 'Show password';
  
      button.onclick = function() {
          if (passwordInput.type === 'password') {
              passwordInput.type = 'text';
          } else {
              passwordInput.type = 'password';
          }
      };
  
      p.appendChild(span);
      p.appendChild(passwordInput);
      p.appendChild(button);
  
      contentDiv.appendChild(p);
  }
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
