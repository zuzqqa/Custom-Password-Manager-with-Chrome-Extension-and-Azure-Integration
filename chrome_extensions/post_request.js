document.getElementById("myForm").addEventListener("submit", function (event) {
  event.preventDefault();
  console.log("Form submitted!");
  var formData = new FormData(this);

  var xhr = new XMLHttpRequest();
  xhr.open("POST", "http://localhost:3000", true);
  xhr.setRequestHeader("Content-Type", "application/json");
  xhr.onload = function () {
    console.log(xhr.responseText);
    if (xhr.status === 200) {
      console.log("success");
    } else {
      console.log("error");
    }
  };
  xhr.send(JSON.stringify(Object.fromEntries(formData.entries())));
});
