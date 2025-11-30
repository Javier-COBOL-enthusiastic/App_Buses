import { storage } from "./storage.js";

// Cambia a true para usar MOCK local
export const USE_MOCK = false;
export const BASE_URL = "http://localhost:5000";

async function req(path, method = "GET", body) {
  const headers = { "Content-Type": "application/json" };
  if (storage.token) headers.Authorization = `Bearer ${storage.token}`;

  const r = await fetch(`${BASE_URL}${path}`, {
    method,
    headers,
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!r.ok) throw new Error(`HTTP ${r.status}`);

  // Algunos endpoints devuelven 204 / 202 sin body
  try {
    return await r.json();
  } catch {
    return null;
  }
}

/* ======== MOCK LOCALSTORAGE ======== */
const USERS_KEY = "bt_users";
const PEPPER = "equipovicturbo";
const keyRoutes = (uid) => `bt_routes_${uid}`;
const keyReset = (email) => `bt_reset_${email.toLowerCase()}`;

function loadUsers() {
  return JSON.parse(localStorage.getItem(USERS_KEY) || "[]");
}
function saveUsers(x) {
  localStorage.setItem(USERS_KEY, JSON.stringify(x));
}

async function sha256(t) {
  const d = new TextEncoder().encode(t);
  const h = await crypto.subtle.digest("SHA-256", d);
  return [...new Uint8Array(h)]
    .map((b) => b.toString(16).padStart(2, "0"))
    .join("");
}
async function hashPass(p) {
  return sha256(p + PEPPER);
}

function loadRoutes(uid) {
  return JSON.parse(localStorage.getItem(keyRoutes(uid)) || "[]");
}
function saveRoutes(uid, arr) {
  localStorage.setItem(keyRoutes(uid), JSON.stringify(arr));
}

export const api = {
  /* ---------- AUTH (/auth) ---------- */

  // POST /auth/register  -> UsuarioRegistroDTO
  async register(payload) {
    const dto = {
      nombre_completo: payload?.nombre_completo ?? payload?.name ?? "",
      correo: payload?.correo ?? payload?.email ?? "",
      usuario: payload?.usuario ?? payload?.username ?? "",
      password: payload?.password ?? "",
    };

    if (USE_MOCK) {
      const users = loadUsers();
      const exists = users.some(
        (u) =>
          u.email.toLowerCase() === dto.correo.toLowerCase() ||
          (dto.usuario &&
            u.username?.toLowerCase() === dto.usuario.toLowerCase())
      );
      if (exists) throw new Error("El usuario ya existe");

      const passHash = await hashPass(dto.password);
      const user = {
        id: Date.now(),
        name: dto.nombre_completo,
        email: dto.correo,
        username: dto.usuario,
        passHash,
        profile: {},
      };
      users.push(user);
      saveUsers(users);
      return { ok: true, user };
    }

    return req("/auth/register", "POST", dto);
  },

  // POST /auth/login -> LoginRequest { usuario, password }
  async login(emailOrUser, password) {
    const usuario =
      typeof emailOrUser === "string"
        ? emailOrUser
        : emailOrUser?.usuario ?? emailOrUser?.email ?? "";

    if (USE_MOCK) {
      const users = loadUsers();
      const u = users.find(
        (x) => x.email.toLowerCase() === usuario.toLowerCase()
      );
      if (!u) throw new Error("Usuario no encontrado");
      const ok = (await hashPass(password)) === u.passHash;
      if (!ok) throw new Error("Credenciales inválidas");
      const rand = crypto.getRandomValues(new Uint32Array(1))[0].toString(16);
      const res = {
        token: "mock-" + rand,
        user: {
          id: u.id,
          name: u.name,
          email: u.email,
          username: u.username,
          profile: u.profile || {},
        },
      };
      storage.token = res.token;
      storage.user = res.user;
      return res;
    }

    const res = await req("/auth/login", "POST", {
      usuario,
      password,
    });

    // Backend: return Results.Ok(new { token = tokenString, user = req.usuario });
    storage.token = res.token;
    storage.user = { usuario: res.user };
    return res;
  },

  /* helpers MOCK de reset local, no hay endpoint real de reset en backend */
  mockUserExists(email) {
    return loadUsers().some(
      (u) => u.email.toLowerCase() === email.toLowerCase()
    );
  },

  issueResetCode(email) {
    const code = Math.floor(100000 + Math.random() * 900000).toString();
    const exp = Date.now() + 10 * 60 * 1000;
    localStorage.setItem(keyReset(email), JSON.stringify({ code, exp }));
    return code;
  },

  async resetPassword(email, code, newPass) {
    const rec = JSON.parse(localStorage.getItem(keyReset(email)) || "null");
    if (!rec) throw new Error("No hay solicitud de reseteo");
    if (Date.now() > rec.exp) throw new Error("Código expirado");
    if (rec.code !== code) throw new Error("Código incorrecto");
    const users = loadUsers();
    const idx = users.findIndex(
      (u) => u.email.toLowerCase() === email.toLowerCase()
    );
    if (idx < 0) throw new Error("Usuario no encontrado");
    users[idx].passHash = await hashPass(newPass);
    saveUsers(users);
    localStorage.removeItem(keyReset(email));
    return { ok: true };
  },

  /* ---------- RUTAS (/ruta) ---------- */

  // GET /buses/ids -> resumen por usuario (cards dashboard)
  async getRoutes() {
    if (USE_MOCK) {
      const uid = storage.user?.id;
      if (!uid) throw new Error("Sin sesión");
      const arr = loadRoutes(uid);
      return arr.map((r) => ({
        id: r.id,
        name: r.name,
        active: r.active || 0,
        inactive: r.inactive || 0,
      }));
    }
    return req("/ruta/ids", "GET");
  },

  // POST /ruta/registrar -> RegistrarRutaDTO { nuevaRuta, coordenadas[] }
  async createRoute(payload) {
    if (USE_MOCK) {
      const uid = storage.user?.id;
      if (!uid) throw new Error("Sin sesión");
      const arr = loadRoutes(uid);
      const r = {
        id: Date.now(),
        name: payload.name,
        code: payload.code || "",
        description: payload.description || "",
        active: 0,
        inactive: 0,
        points: payload.points || [],
        teams: [],
      };
      arr.push(r);
      saveRoutes(uid, arr);
      return r;
    }

    const dto = {
      nuevaRuta: {
        nombre_ruta: payload.nombre_ruta ?? payload.name ?? "",
        descripcion_ruta:
          payload.descripcion_ruta ?? payload.description ?? "",
      },
      coordenadas: (payload.coordenadas ?? payload.markers ?? []).map((m) => ({
        latitud: m.latitud ?? m.lat ?? 0,
        longitud: m.longitud ?? m.lng ?? 0,
      })),
    };

    return req("/ruta/registrar", "POST", dto);
  },

  // POST /ruta/registrar/{idRuta:int} -> vincular ruta existente al usuario
  async linkExistingRoute(idRuta) {
    if (USE_MOCK) {
      throw new Error("linkExistingRoute solo aplica con backend real.");
    }
    return req(`/ruta/registrar/${idRuta}`, "POST");
  },

  // DELETE /ruta/eliminar/{idRuta:int}
  async deleteRoute(idRuta) {
    if (USE_MOCK) {
      const uid = storage.user?.id;
      let arr = loadRoutes(uid);
      arr = arr.filter((x) => x.id != idRuta);
      saveRoutes(uid, arr);
      return { ok: true };
    }
    return req(`/ruta/eliminar/${idRuta}`, "DELETE");
  },

  // GET /ruta/{idRuta:int}
  // GET /ruta/coordenadas/{idRuta:int}
  // GET /buses/ids
  // GET /choferes/get
  async getALLRoutes() {
    const rutasIDs = await req(`/ruta/allids`, "GET");
    return rutasIDs
  },

  async getRouteDetail(idRuta) {
    if (USE_MOCK) {
      const uid = storage.user?.id;
      const arr = loadRoutes(uid);
      const r = arr.find((x) => x.id == idRuta);
      if (!r) throw new Error("Ruta no existe");
      return {
        id: r.id,
        name: r.name,
        code: r.code || "",
        description: r.description || "",
        active: r.active || 0,
        inactive: r.inactive || 0,
        teams: r.teams || [],
        markers: r.points || [],
      };
    }

    const [ruta, coords, buses, choferes] = await Promise.all([
      req(`/ruta/${idRuta}`, "GET"),
      req(`/ruta/coordenadas/${idRuta}`, "GET"),
      req("/buses/ids", "GET"),
      req("/choferes/get", "GET"),
    ]);

    for (let i = 0; i < buses.length; i++) {
      buses[i] = await this.getBusById(buses[i]);
    }

    const busesRuta = (buses || []).filter((b) => b.id_ruta === idRuta);

    //console.log(busesRuta);

    const teams = busesRuta.map((bus) => {
      const chofer = (choferes || []).find(
        (c) => c.id_bus === bus.id_bus
      );
      return {
        id: bus.id_bus,
        id_bus: bus.id_bus,
        id_ruta: bus.id_ruta,
        id_usuario: bus.id_usuario,
        id_chofer: chofer?.id_chofer ?? null,
        name: bus.numero_placa,
        driver: chofer?.nombre_completo || "—",
        phone: chofer?.telefono_chofer || "—",
        active: !!bus.estado_bus,
      };
    });

    const active = teams.filter((t) => t.active).length;
    const inactive = teams.length - active;

    return {
      id: ruta.id_ruta,
      name: ruta.nombre_ruta,
      description: ruta.descripcion_ruta,
      active,
      inactive,
      teams,
      markers: coords || [],
    };
  },

  /* ---------- BUSES / TEAMS (/buses) ---------- */

  // GET /buses/get/{id:int}
  async getBusById(idBus) {
    return req(`/buses/get/${idBus}`, "GET");
  },

  // POST /buses/add  +  POST /choferes/add
  async addTeam(routeId, payload) {
    if (USE_MOCK) {
      const uid = storage.user?.id;
      const arr = loadRoutes(uid);
      const i = arr.findIndex((x) => x.id == routeId);
      if (i < 0) throw new Error("Ruta no existe");
      const t = {
        id: Date.now(),
        name:
          payload.name ||
          `Equipo ${(arr[i].teams?.length || 0) + 1}`,
        driver: payload.driver || "—",
        phone: payload.phone || "—",
        active: true,
      };
      arr[i].teams = arr[i].teams || [];
      arr[i].teams.push(t);
      arr[i].active = (arr[i].active || 0) + 1;
      saveRoutes(uid, arr);
      return t;
    }

    const busDto = {
      numero_placa: payload.name ?? "",
      estado_bus: payload.active ?? true,
      id_ruta: routeId,
      id_usuario: 0, // se obtiene en el servicio desde el JWT
    };

    await req("/buses/add", "POST", busDto);

    const detailAfterBus = await api.getRouteDetail(routeId);
    const lastTeam =
      detailAfterBus.teams[detailAfterBus.teams.length - 1];

    if (payload.driver || payload.phone) {
      const choferDto = {
        nombre_completo: payload.driver || "",
        telefono_chofer: payload.phone || "",
        id_bus: lastTeam.id_bus,
      };
      await req("/choferes/add", "POST", choferDto);
    }

    const finalDetail = await api.getRouteDetail(routeId);
    return finalDetail.teams.find((t) => t.id === lastTeam.id) || lastTeam;
  },

  // PUT /buses/update  +  PUT /choferes/update / POST /choferes/add
  async updateTeam(routeId, teamId, fields) {
    if (USE_MOCK) {
      const uid = storage.user?.id;
      const arr = loadRoutes(uid);
      const i = arr.findIndex((x) => x.id == routeId);
      if (i < 0) throw new Error("Ruta no existe");
      const j = (arr[i].teams || []).findIndex((t) => t.id == teamId);
      if (j < 0) throw new Error("Equipo no existe");
      arr[i].teams[j] = { ...arr[i].teams[j], ...fields };
      saveRoutes(uid, arr);
      return arr[i].teams[j];
    }

    const detail = await api.getRouteDetail(routeId);
    const team = detail.teams.find((t) => t.id === teamId);
    if (!team) throw new Error("Bus no encontrado para esta ruta");

    const updated = { ...team, ...fields };

    const busPayload = {
      id_bus: team.id_bus,
      numero_placa: updated.name,
      estado_bus: !!updated.active,
      id_ruta: routeId,
      id_usuario: team.id_usuario ?? 0,
    };
    await req("/buses/update", "PUT", busPayload);

    if (updated.driver || updated.phone) {
      if (team.id_chofer) {
        const choferPayload = {
          id_chofer: team.id_chofer,
          nombre_completo: updated.driver || "",
          telefono_chofer: updated.phone || "",
          id_bus: team.id_bus,
        };
        await req("/choferes/update", "PUT", choferPayload);
      } else {
        const choferDto = {
          nombre_completo: updated.driver || "",
          telefono_chofer: updated.phone || "",
          id_bus: team.id_bus,
        };
        await req("/choferes/add", "POST", choferDto);
      }
    }

    const finalDetail = await api.getRouteDetail(routeId);
    return finalDetail.teams.find((t) => t.id === teamId) || updated;
  },

  // DELETE /buses/delete/{id:int} + /choferes/delete/{id:int}
  async deleteTeam(routeId, teamId) {
    if (USE_MOCK) {
      const uid = storage.user?.id;
      const arr = loadRoutes(uid);
      const i = arr.findIndex((x) => x.id == routeId);
      if (i < 0) throw new Error("Ruta no existe");
      const team = (arr[i].teams || []).find((t) => t.id == teamId);
      arr[i].teams = (arr[i].teams || []).filter((t) => t.id != teamId);
      if (team) {
        if (team.active) {
          arr[i].active = Math.max(0, (arr[i].active || 0) - 1);
        } else {
          arr[i].inactive = Math.max(0, (arr[i].inactive || 0) - 1);
        }
      }
      saveRoutes(uid, arr);
      return { ok: true };
    }

    const detail = await api.getRouteDetail(routeId);
    const team = detail.teams.find((t) => t.id === teamId);
    if (!team) throw new Error("Bus no encontrado para esta ruta");

    if (team.id_chofer) {
      await req(`/choferes/delete/${team.id_chofer}`, "DELETE");
    }
    await req(`/buses/delete/${team.id_bus}`, "DELETE");

    return { ok: true };
  },

  // PATCH lógico: toggle activo/inactivo usando PUT /buses/update
  async toggleTeam(routeId, teamId, isActive) {
    if (USE_MOCK) {
      const uid = storage.user?.id;
      const arr = loadRoutes(uid);
      const i = arr.findIndex((x) => x.id == routeId);
      if (i < 0) throw new Error("Ruta no existe");
      const j = arr[i].teams.findIndex((t) => t.id == teamId);
      if (j < 0) throw new Error("Equipo no existe");
      const prev = !!arr[i].teams[j].active;
      arr[i].teams[j].active = !!isActive;
      if (prev !== isActive) {
        if (isActive) {
          arr[i].active++;
          arr[i].inactive = Math.max(0, (arr[i].inactive || 0) - 1);
        } else {
          arr[i].inactive = (arr[i].inactive || 0) + 1;
          arr[i].active = Math.max(0, arr[i].active - 1);
        }
      }
      saveRoutes(uid, arr);
      return arr[i].teams[j];
    }

    const detail = await api.getRouteDetail(routeId);
    const team = detail.teams.find((t) => t.id === teamId);
    if (!team) throw new Error("Bus no encontrado para esta ruta");

    const busPayload = {
      id_bus: team.id_bus,
      numero_placa: team.name,
      estado_bus: !!isActive,
      id_ruta: routeId,
      id_usuario: team.id_usuario ?? 0,
    };
    await req("/buses/update", "PUT", busPayload);

    const finalDetail = await api.getRouteDetail(routeId);
    return (
      finalDetail.teams.find((t) => t.id === teamId) || {
        ...team,
        active: !!isActive,
      }
    );
  },

  /* ---------- CHOFERES (/choferes) ---------- */

  // GET /choferes/get
  async listDrivers() {
    return req("/choferes/get", "GET");
  },

  // GET /choferes/get/{id:int}
  async getDriverById(idChofer) {
    return req(`/choferes/get/${idChofer}`, "GET");
  },

  /* ---------- SNAP TO ROAD (/ruta/snap-to-road) ---------- */

  // POST /ruta/snap-to-road
  async snapToRoad(lat, lng) {
    return req("/ruta/snap-to-road", "POST", {
      latitud: lat,
      longitud: lng,
    });
  },
};
