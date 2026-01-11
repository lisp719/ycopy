document.addEventListener("DOMContentLoaded", function () {
  document.querySelectorAll('[data-action="copy"]').forEach((button) => {
    button.addEventListener("click", async function () {
      const url = this.dataset.url;
      if (url) {
        try {
          await navigator.clipboard.writeText(url);
          console.log("URL copied to clipboard:", url);
        } catch (err) {
          console.error("Failed to copy URL:", err);
        }
      }
    });
  });
});
