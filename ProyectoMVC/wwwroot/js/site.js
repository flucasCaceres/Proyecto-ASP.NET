// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
  // ─────────────────────────────────────────────────────────────────────────
  // 1) Función para enganchar el submit del form de login
  // ─────────────────────────────────────────────────────────────────────────
  // auth-modal.js

// Engancha el formulario de login
// auth-modal.js

// Función genérica para enganchar formularios de login/registro
function engancharForm({ formId, postUrl, redirectUrl }) {
  const form = document.getElementById(formId);
  if (!form) return;

  // Remover listeners antiguos clonando el nodo
  form.replaceWith(form.cloneNode(true));
  const nuevoForm = document.getElementById(formId);

  nuevoForm.addEventListener("submit", async e => {
    e.preventDefault();
    const formData = new FormData(nuevoForm);

    const resp = await fetch(postUrl, {
      method: "POST",
      body: formData,
      headers: {
        "RequestVerificationToken": formData.get("__RequestVerificationToken")
      }
    });
    const tipo = resp.headers.get("content-type") || "";

    if (tipo.includes("application/json")) {
      const data = await resp.json();
      if (data.success) {
        bootstrap.Modal.getInstance(document.getElementById("authModal")).hide();
        window.location.href = redirectUrl;
        return;
      }
    }

    // Si no es JSON, vino HTML con validaciones: reinyectar y reenganchar
    const htmlErr = await resp.text();
    document.getElementById("modal-content-container").innerHTML = htmlErr;
    engancharForm({ formId, postUrl, redirectUrl });
  });
}

// Abre el modal y carga el partial, luego engancha el formulario correspondiente
async function abrirAuthModal(ruta) {
  const modalEl = document.getElementById("authModal");
  const viejo = bootstrap.Modal.getInstance(modalEl);
  if (viejo) {
    viejo.hide();
    document.body.classList.remove("modal-open");
    document.querySelectorAll(".modal-backdrop").forEach(el => el.remove());
  }

  setTimeout(async () => {
    const resp = await fetch(ruta);
    if (!resp.ok) return;
    const html = await resp.text();
    document.getElementById("modal-content-container").innerHTML = html;
    new bootstrap.Modal(modalEl).show();

    if (ruta.includes("/Login")) {
      engancharForm({
        formId:       "loginForm",
        postUrl:      "/Account/Login",
        redirectUrl:  "/Home/Index"
      });
    } else if (ruta.includes("/Register")) {
      engancharForm({
        formId:       "registerForm",
        postUrl:      "/Account/Register",
        redirectUrl:  "/Privacy/Privacy"
      });
    }
  }, 150);
}

// Listener para los botones de login y registro
document.addEventListener("DOMContentLoaded", () => {
  const btnLogin = document.getElementById("btnAbrirLogin");
  const btnReg   = document.getElementById("btnAbrirRegister");

  if (btnLogin) btnLogin.addEventListener("click", () => abrirAuthModal("/Account/Login"));
  if (btnReg)   btnReg.addEventListener("click", () => abrirAuthModal("/Account/Register"));
});