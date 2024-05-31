function redirectToView(viewPath) {
  fetch(chrome.runtime.getURL(viewPath))
    .then((response) => response.text())
    .then((html) => {
      var oldScripts = document.getElementsByTagName("script");
      for (let i = oldScripts.length - 1; i >= 0; i--) {
        oldScripts[i].parentNode.removeChild(oldScripts[i]);
      }

      var tempDiv = document.createElement("div");
      tempDiv.innerHTML = html;

      var scripts = tempDiv.getElementsByTagName("script");
      var scriptSources = Array.from(scripts).map((script) => script.src);

      document.body.innerHTML = html;

      scriptSources.forEach((src) => {
        var script = document.createElement("script");
        script.src = src;
        document.body.appendChild(script);
      });
    })
    .catch((error) => {
      console.warn("Something went wrong.", error);
    });
}
