import { setThemeToggle, toast, $ } from "./ui.js";
import { api } from "./api.js";
import { storage } from "./storage.js";

if (!storage.token) location.href = "./login.html";

setThemeToggle(document.getElementById("themeBtn"));

const map = L.map("map").setView([13.69294, -89.21819], 12);
L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",{maxZoom:19, attribution:"© OpenStreetMap"}).addTo(map);

const params = new URLSearchParams(location.search);
const routeId = params.get("id") || 1;

async function loadRoute(){
  const data = await api.getRouteDetail(routeId);
  $("#routeTitle").textContent = data.name;
  const tbody = document.querySelector("#teamsTable tbody");
  tbody.innerHTML = data.teams.map(t=>`<tr><td>${t.name}</td><td>—</td><td>—</td></tr>`).join("");
  data.markers.forEach(p=> L.marker([p.lat,p.lng]).addTo(map));
  document.getElementById("countActive").textContent = Math.max(1, data.markers.length - 1);
  document.getElementById("countInactive").textContent = 1;
}

document.getElementById("csvInput").addEventListener("change", e=>{
  const file = e.target.files?.[0]; if(!file) return;
  const fr = new FileReader();
  fr.onload = () => {
    const lines = String(fr.result).trim().split("\n");
    const pts = [];
    for(let i=1;i<lines.length;i++){
      const c = lines[i].split(",").map(s=>s.trim());
      let lat = parseFloat(c[0]), lon = parseFloat(c[1]);
      if(Number.isNaN(lat) || Number.isNaN(lon)){ lat = parseFloat(c[1]); lon = parseFloat(c[2]); }
      if(!Number.isNaN(lat) && !Number.isNaN(lon)) pts.push([lat,lon]);
    }
    if(pts.length){ L.polyline(pts).addTo(map); map.fitBounds(pts); toast("Ruta cargada desde CSV","ok"); }
    else{ toast("CSV sin puntos válidos","bad"); }
  };
  fr.readAsText(file);
});

document.getElementById("btnAddTeam").addEventListener("click", ()=> {
  document.getElementById("modalTeam").classList.add("open");
});
document.getElementById("formTeam").addEventListener("submit", async (e)=>{
  e.preventDefault();
  try{
    await api.addTeam(routeId, {
      colors: document.getElementById("tColors").value.trim(),
      driver: document.getElementById("tDriver").value.trim(),
      phone:  document.getElementById("tPhone").value.trim()
    });
    toast("Equipo agregado correctamente","ok");
    document.getElementById("modalTeam").classList.remove("open");
  }catch{ toast("Error al agregar equipo","bad"); }
});

loadRoute();
