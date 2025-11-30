import { setThemeToggle } from "./ui.js";
import { storage } from "./storage.js";

export function renderHeader(opts={}){
  const el = document.getElementById("appHeader"); if(!el) return;
  el.innerHTML = `
  <header class="appbar">
    <div class="logo">
      <img class="logo-ico" src="./assets/img/logo-bus.png" alt="logo" width="26" height="26">
      <strong>BusTrackSV</strong>
    </div>
    <div class="grow"></div>
    ${opts.left || ""}
    <span class="badge">El Salvador</span>
    <button class="icon-btn" id="themeBtn" title="Tema"></button>
    ${opts.back?`<a class="btn ghost" href="${opts.back}">Volver</a>`:""}
    ${opts.logout?`<a class="btn ghost" id="logoutBtn" href="#">Salir</a>`:""}
  </header>`;
  setThemeToggle(document.getElementById("themeBtn"));
  if(opts.logout){
    document.getElementById("logoutBtn").addEventListener("click",(e)=>{ e.preventDefault(); storage.logout(); location.href="./login.html"; });
  }
}
export function renderFooter(){
  const el = document.getElementById("appFooter"); if(!el) return;
  el.innerHTML = `<footer class="footer">&copy; 2025 BusTrackSV</footer>`;
}
