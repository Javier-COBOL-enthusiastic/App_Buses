import { setThemeToggle, toast, $ } from "./ui.js";
import { api } from "./api.js";
import { storage } from "./storage.js";

if (!storage.token) location.href = "./login.html";

setThemeToggle(document.getElementById("themeBtn"));

const map = L.map("map").setView([13.69294, -89.21819], 12);
L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", { maxZoom: 19, attribution: "© OpenStreetMap" }).addTo(map);

const params = new URLSearchParams(location.search);
const routeId = params.get("id") || 1;

async function loadRoute() {
  const data = await api.getRouteDetail(routeId);
  $("#routeTitle").textContent = data.name;
  const tbody = document.querySelector("#teamsTable tbody");
  tbody.innerHTML = data.teams.map(t => `<tr><td>${t.name}</td><td>—</td><td>—</td></tr>`).join("");
  data.markers.forEach(p => L.marker([p.lat, p.lng]).addTo(map));
  document.getElementById("countActive").textContent = Math.max(1, data.markers.length - 1);
  document.getElementById("countInactive").textContent = 1;
}

document.getElementById("btnAddTeam").addEventListener("click", () => {
  document.getElementById("modalTeam").classList.add("open");
});
document.getElementById("formTeam").addEventListener("submit", async (e) => {
  e.preventDefault();
  try {
    await api.addTeam(routeId, {
      colors: document.getElementById("tColors").value.trim(),
      driver: document.getElementById("tDriver").value.trim(),
      phone: document.getElementById("tPhone").value.trim()
    });
    toast("Equipo agregado correctamente", "ok");
    document.getElementById("modalTeam").classList.remove("open");
  } catch { toast("Error al agregar equipo", "bad"); }
});

loadRoute();
