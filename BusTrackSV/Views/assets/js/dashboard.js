  import { renderHeader, renderFooter } from "./components.js";
  import { openModal, closeModal, toast } from "./ui.js";
  import { api, HaversineDistance } from "./api.js";
  import { storage } from "./storage.js";

  // ───────────────────────────────────
  //   Setup básico
  // ───────────────────────────────────

  if (!storage.token) location.href = "login.html";
  renderHeader({ logout: true });
  renderFooter();

  let deleteMode = false;
  let pendingRouteId = null;

  const btnNew = document.getElementById("btnNuevaRuta");
  const btnDel = document.getElementById("btnToggleDelete");

  // ───────────────────────────────────
  //   Tabla de rutas (usa api.getRoutes -> GET /buses/ids)
  // ───────────────────────────────────

  function setToolbarState() {
    if (deleteMode) {
      btnNew.classList.add("hidden");
      btnDel.textContent = "Cancelar";
    } else {
      btnNew.classList.remove("hidden");
      btnDel.textContent = "Eliminar rutas";
    }
  }

  function renderRows(routes) {
    const tbody = document.querySelector("#routesTable tbody");
    if (!routes || !routes.length) {
      tbody.innerHTML =
        '<tr><td colspan="5" class="muted">Aún no hay rutas registradas</td></tr>';
      return;
    }

    tbody.innerHTML = routes
      .map((r) => {
        // intentamos ser tolerantes con nombres de propiedades
        const id =
          r.id ?? r.id_ruta ?? r.idRuta ?? r.id_route ?? r.id_bus ?? r.idBus;
        const name = r.name ?? r.nombre_ruta ?? r.routeName ?? `Ruta ${id}`;
        const active = r.active ?? r.activos ?? r.activosAhora ?? 0;
        const inactive = r.inactive ?? r.inactivos ?? 0;
        const total = active + inactive;

        const actionCell = deleteMode
          ? `<button class="btn danger" data-act="del">Eliminar</button>`
          : `<a class="btn ghost" href="rutas.html?id=${encodeURIComponent(
            id
          )}">Abrir</a>`;

        return `<tr data-id="${id}">
          <td><strong>${name}</strong></td>
          <td><span class="pill ok">${active}</span></td>
          <td><span class="pill bad">${inactive}</span></td>
          <td>${total}</td>
          <td class="row" style="gap:.5rem">${actionCell}</td>
        </tr>`;
      })
      .join("");

    if (deleteMode) {
      document.querySelectorAll("[data-act='del']").forEach((b) => {
        b.addEventListener("click", (e) => {
          const tr = e.currentTarget.closest("tr");
          pendingRouteId = Number(tr.dataset.id);
          document.getElementById("confirmText").textContent =
            "¿Eliminar esta ruta? Esta acción no se puede deshacer.";
          openModal("modalConfirm");
        });
      });
    }
  }

  async function loadRoutes() {
    try {
      const routesID = await api.getRoutes(); // GET /ruta/ids
      const routesDetail = [];
      for (const r of routesID) {
        const detail = await api.getRouteDetail(r); // precachear detalles      
        routesDetail.push(detail);
      }

      renderRows(routesDetail || []);
    } catch (err) {
      console.error(err);
      toast("Error al cargar las rutas", "bad");
    }
  }

  // ───────────────────────────────────
  //   Menú principal "Registrar nueva ruta"
  // ───────────────────────────────────

  const modalTipoId = "modalTipoRuta";
  const modalExistenteId = "modalRutaExistente";
  const modalNuevaId = "modalRutaNueva";

  // botones del menú tipo
  const btnTipoExistente = document.getElementById("btnTipoExistente");
  const btnTipoNueva = document.getElementById("btnTipoNueva");
  const btnTipoCancelar = document.getElementById("btnTipoCancelar");

  // ruta existente
  const selectRutaExistente = document.getElementById("selectRutaExistente");
  const btnGuardarRutaExistente = document.getElementById(
    "btnGuardarRutaExistente"
  );
  const btnCancelarRutaExistente = document.getElementById(
    "btnCancelarRutaExistente"
  );

  // nueva ruta
  const inputNombreNueva = document.getElementById("rNewName");
  const inputDescNueva = document.getElementById("rNewDesc");
  const btnGuardarRutaNueva = document.getElementById("btnGuardarRutaNueva");
  const btnCancelarRutaNueva = document.getElementById("btnCancelarRutaNueva");

  // abrir menú al click en "Registrar nueva ruta"
  btnNew.addEventListener("click", () => openModal(modalTipoId));

  // cerrar menú
  btnTipoCancelar.addEventListener("click", () => closeModal(modalTipoId));

  // ───────────────────────────────────
  //   Registrar RUTA EXISTENTE
  //   -> usa api.linkExistingRoute(idRuta) -> POST /ruta/registrar/{idRuta}
  // ───────────────────────────────────

  btnTipoExistente.addEventListener("click", async () => {
    closeModal(modalTipoId);
    openModal(modalExistenteId);
    await cargarRutasExistentes();
  });

  btnCancelarRutaExistente.addEventListener("click", () => {
    closeModal(modalExistenteId);
  });

  async function cargarRutasExistentes() {
    selectRutaExistente.innerHTML =
      "<option value=''>Cargando rutas...</option>";
    try {
      // Por ahora usamos el mismo endpoint del dashboard.
      const rutasID = await api.getALLRoutes();
      const rutasIDregistradas = await api.getRoutes();

      // Filtrar las rutas que NO estan registradas
      const rutasIDfiltradas = rutasID.filter(r => !rutasIDregistradas.includes(r));

      const rutas = [];
      for (const r of rutasIDfiltradas) {
        const detail = await api.getRouteDetail(r);
        rutas.push(detail);
      }

      if (!rutas || !rutas.length) {
        selectRutaExistente.innerHTML =
          "<option value=''>No hay rutas disponibles</option>";
        return;
      }

      selectRutaExistente.innerHTML =
        "<option value=''>Seleccioná una ruta...</option>";

      for (const r of rutas) {
        const id =
          r.id ?? r.id_ruta ?? r.idRuta ?? r.id_route ?? r.id_bus ?? r.idBus;
        const name = r.name ?? r.nombre_ruta ?? r.routeName ?? `Ruta ${id}`;
        const opt = document.createElement("option");
        opt.value = id;
        opt.textContent = name;
        selectRutaExistente.appendChild(opt);
      }
    } catch (err) {
      console.error(err);
      selectRutaExistente.innerHTML =
        "<option value=''>Error al cargar rutas</option>";
    }
  }

  btnGuardarRutaExistente.addEventListener("click", async () => {
    const idRuta = Number(selectRutaExistente.value);
    if (!idRuta) {
      toast("Seleccioná una ruta", "bad");
      return;
    }

    try {
      await api.linkExistingRoute(idRuta); // POST /ruta/registrar/{idRuta}
      toast("Ruta vinculada correctamente", "ok");
      closeModal(modalExistenteId);
      loadRoutes();
    } catch (err) {
      console.error(err);
      toast("No se pudo vincular la ruta", "bad");
    }
  });

  // ───────────────────────────────────
  //   Registrar NUEVA RUTA con Leaflet
  //   -> usa api.createRoute(payload) -> POST /ruta/registrar
  // ───────────────────────────────────

  btnTipoNueva.addEventListener("click", () => {
    closeModal(modalTipoId);
    openModal(modalNuevaId);
    initMapNuevaRuta();
  });

  btnCancelarRutaNueva.addEventListener("click", () => {
    closeModal(modalNuevaId);
    resetNuevaRuta();
  });

  let mapNuevaRuta;
  let puntosRuta = [];
  let polylineRuta = null;
  let markersRuta = [];

  function initMapNuevaRuta() {
    if (mapNuevaRuta) {
      // si ya existe (abriste y cerraste el modal), solo reajustamos
      setTimeout(() => mapNuevaRuta.invalidateSize(), 200);
      return;
    }

    mapNuevaRuta = L.map("mapNuevaRuta").setView([13.69294, -89.21819], 12);
    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      maxZoom: 19,
      attribution: "© OpenStreetMap",
    }).addTo(mapNuevaRuta);

    mapNuevaRuta.on("click", async (e) => {
      const clickedPt = e.latlng;

      try {
        // Llamar al backend para ajustar el punto a la calle más cercana
        const snappedPt = await api.snapToRoad(clickedPt.lat, clickedPt.lng);
        
        
        if(markersRuta.length > 0) {        
          const lastPt = puntosRuta[puntosRuta.length - 1];      
                    
          console.log("Punto ajustado:", snappedPt);
          console.log("Último punto:", lastPt);        
          const distance = HaversineDistance(
            lastPt.latitud,
            lastPt.longitud,
            snappedPt.latitud,
            snappedPt.longitud
          );          
          
          if(distance > 300) {
            toast
            (
            `Error: El punto está a ${Math.round(distance)}m del anterior. Máximo permitido: 300m.`,
            "bad"
            );
            return;
          }
        }

        puntosRuta.push(snappedPt);

        // Mostrar marcador en el punto ajustado
        const marker = L.circleMarker([snappedPt.latitud, snappedPt.longitud], {
          radius: 4,
          color: "#3388ff",
          fillColor: "#3388ff",
          fillOpacity: 0.8,
        });
        marker.addTo(mapNuevaRuta);
        
        markersRuta.push(marker);
        
        marker.on("contextmenu", () => {    
          const index = markersRuta.indexOf(marker);
          if (index !== -1) {        
            mapNuevaRuta.removeLayer(marker);

            markersRuta.splice(index, 1);
            puntosRuta.splice(index, 1);

            const newPoints = puntosRuta.map(p => [p.latitud, p.longitud]);
          
            if (polylineRuta) {
              polylineRuta.setLatLngs(newPoints);
            }
          }
        });

        const linePoints = puntosRuta.map((p) => [p.latitud, p.longitud]);
        if (polylineRuta) {
          polylineRuta.setLatLngs(linePoints);
        } else {
          polylineRuta = L.polyline(linePoints, {
            color: "#3388ff",
            weight: 3,
          }).addTo(mapNuevaRuta);
        }
      } catch (err) {
        console.error("Error al ajustar punto:", err);
        toast("Error al ajustar punto a la calle", "bad");
      }
    });


  }

  function resetNuevaRuta() {
    puntosRuta = [];
    if (mapNuevaRuta) {
      if (polylineRuta) {
        mapNuevaRuta.removeLayer(polylineRuta);
        polylineRuta = null;
      }
      markersRuta.forEach((m) => mapNuevaRuta.removeLayer(m));
      markersRuta = [];
    }
    inputNombreNueva.value = "";
    inputDescNueva.value = "";
  }

  btnGuardarRutaNueva.addEventListener("click", async () => {
    const nombre = inputNombreNueva.value.trim();
    const descripcion = inputDescNueva.value.trim();

    if (!nombre) {
      toast("Poné un nombre / código para la ruta", "bad");
      return;
    }
    if (puntosRuta.length < 2) {
      toast("Necesitás al menos dos puntos en el mapa", "bad");
      return;
    }

    const payload = {
      nombre_ruta: nombre,
      descripcion_ruta: descripcion,
      // api.createRoute ya hace mapeo {lat,lng} -> {latitud,longitud}
      coordenadas: puntosRuta,
    };

    try {
      await api.createRoute(payload); // POST /ruta/registrar
      toast("Ruta registrada correctamente", "ok");
      closeModal(modalNuevaId);
      resetNuevaRuta();
      loadRoutes();
    } catch (err) {
      console.error(err);
      toast("Error al registrar la ruta", "bad");
    }
  });

  // ───────────────────────────────────
  //   Eliminar rutas (usa api.deleteRoute -> DELETE /ruta/eliminar/{idRuta})
  // ───────────────────────────────────

  btnDel.addEventListener("click", () => {
    deleteMode = !deleteMode;
    setToolbarState();
    loadRoutes();
  });

  document.getElementById("confirmNo").addEventListener("click", () => {
    closeModal("modalConfirm");
  });

  document.getElementById("confirmYes").addEventListener("click", async () => {
    if (pendingRouteId) {
      try {
        await api.deleteRoute(pendingRouteId); // DELETE /ruta/eliminar/{idRuta}
        toast("Ruta eliminada", "ok");
      } catch (err) {
        console.error(err);
        toast("No se pudo eliminar", "bad");
      }
      pendingRouteId = null;
      closeModal("modalConfirm");
      loadRoutes();
    }
  });

  // arrancar
  loadRoutes();
  setToolbarState();


  