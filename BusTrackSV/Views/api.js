import { storage } from "./storage.js";

// Cambia a false cuando conectes backend real
export const USE_MOCK = false;
export const BASE_URL = "http://localhost:5000";

async function req(path, method="GET", body){
  const headers={"Content-Type":"application/json"};
  if(storage.token) headers.Authorization = `Bearer ${storage.token}`;
  const r = await fetch(`${BASE_URL}${path}`, {method, headers, body: body?JSON.stringify(body):undefined});
  if(!r.ok) throw new Error(`HTTP ${r.status}`);
  return r.json();
}

/* ======== MOCK LOCALSTORAGE ======== */
const USERS_KEY = "bt_users";
const PEPPER = "equipovicturbo";
const keyRoutes = uid => `bt_routes_${uid}`;
const keyReset = email => `bt_reset_${email.toLowerCase()}`;

function loadUsers(){ return JSON.parse(localStorage.getItem(USERS_KEY)||"[]"); }
function saveUsers(x){ localStorage.setItem(USERS_KEY, JSON.stringify(x)); }

async function sha256(t){ const d=new TextEncoder().encode(t); const h=await crypto.subtle.digest("SHA-256",d); return [...new Uint8Array(h)].map(b=>b.toString(16).padStart(2,"0")).join(""); }
async function hashPass(p){ return sha256(p+PEPPER); }

function loadRoutes(uid){ return JSON.parse(localStorage.getItem(keyRoutes(uid))||"[]"); }
function saveRoutes(uid,arr){ localStorage.setItem(keyRoutes(uid), JSON.stringify(arr)); }

export const api = {
  /* ---------- AUTH ---------- */
  async register({name,email,username,password,profile}){
    if(USE_MOCK){
      const users = loadUsers();
      const exists = users.some(u => u.email.toLowerCase()===email.toLowerCase() || (username && u.username?.toLowerCase()===username.toLowerCase()));
      if(exists) throw new Error("El usuario ya existe");
      const passHash = await hashPass(password);
      const user = { id: Date.now(), name, email, username, passHash, profile: profile||{} };
      users.push(user); saveUsers(users);
      return { ok:true, user };
    }
    let id = 1;
    return req("/auth/register","POST",{id, name,email,username,password});
  },
  async login(usuario,password){
    if(USE_MOCK){
      const users = loadUsers();
      const u = users.find(x=> x.email.toLowerCase()===email.toLowerCase());
      if(!u) throw new Error("Usuario no encontrado");
      const ok = (await hashPass(password)) === u.passHash;
      if(!ok) throw new Error("Credenciales inválidas");
      const rand = crypto.getRandomValues(new Uint32Array(1))[0].toString(16);
      return { token:"mock-"+rand, user:{id:u.id,name:u.name,email:u.email,username:u.username,profile:u.profile||{}} };
    }
    return req("/auth/login","POST",{usuario,password});
  },
  mockUserExists(email){ return loadUsers().some(u=>u.email.toLowerCase()===email.toLowerCase()); },
  issueResetCode(email){
    const code = Math.floor(100000 + Math.random()*900000).toString();
    const exp = Date.now() + 10*60*1000;
    localStorage.setItem(keyReset(email), JSON.stringify({code,exp}));
    return code;
  },
  async resetPassword(email, code, newPass){
    const rec = JSON.parse(localStorage.getItem(keyReset(email))||"null");
    if(!rec) throw new Error("No hay solicitud de reseteo");
    if(Date.now()>rec.exp) throw new Error("Código expirado");
    if(rec.code!==code) throw new Error("Código incorrecto");
    const users = loadUsers();
    const idx = users.findIndex(u=>u.email.toLowerCase()===email.toLowerCase());
    if(idx<0) throw new Error("Usuario no encontrado");
    users[idx].passHash = await hashPass(newPass);
    saveUsers(users);
    localStorage.removeItem(keyReset(email));
    return {ok:true};
  },

  /* ---------- ROUTES ---------- */
  async getRoutes(){
    if(USE_MOCK){
      const uid = storage.user?.id; if(!uid) throw new Error("Sin sesión");
      const arr = loadRoutes(uid);
      return arr.map(r=>({ id:r.id, name:r.name, active:r.active||0, inactive:r.inactive||0 }));
    }
    return req("/routes");
  },
  async createRoute(payload){
    if(USE_MOCK){
      const uid = storage.user?.id; if(!uid) throw new Error("Sin sesión");
      const arr = loadRoutes(uid);
      const r = { id: Date.now(), name: payload.name, code: payload.code||"", description: payload.description||"", active:0, inactive:0, points:[], teams:[] };
      arr.push(r); saveRoutes(uid,arr);
      return r;
    }
    return req("/routes","POST",payload);
  },
  async updateRoute(id, fields){
    if(USE_MOCK){
      const uid = storage.user?.id; const arr = loadRoutes(uid);
      const i = arr.findIndex(x=>x.id==id); if(i<0) throw new Error("Ruta no existe");
      arr[i] = { ...arr[i], ...fields }; saveRoutes(uid,arr); return arr[i];
    }
    return req(`/routes/${id}`,"PUT",fields);
  },
  async deleteRoute(id){
    if(USE_MOCK){
      const uid = storage.user?.id; let arr = loadRoutes(uid);
      arr = arr.filter(x=>x.id!=id); saveRoutes(uid,arr); return {ok:true};
    }
    return req(`/routes/${id}`,"DELETE");
  },
  async getRouteDetail(id){
    if(USE_MOCK){
      const uid = storage.user?.id; const arr = loadRoutes(uid);
      const r = arr.find(x=>x.id==id); if(!r) throw new Error("Ruta no existe");
      return { id:r.id, name:r.name, code:r.code||"", description:r.description||"", active:r.active||0, inactive:r.inactive||0, teams:r.teams||[], markers:r.points||[] };
    }
    return req(`/routes/${id}`);
  },
  async addTeam(routeId, payload){
    if(USE_MOCK){
      const uid = storage.user?.id; const arr = loadRoutes(uid);
      const i = arr.findIndex(x=>x.id==routeId); if(i<0) throw new Error("Ruta no existe");
      const t = { id: Date.now(), name: payload.name || `Equipo ${ (arr[i].teams?.length||0)+1 }`, driver: payload.driver||"—", phone: payload.phone||"—", active: true };
      arr[i].teams = arr[i].teams||[]; arr[i].teams.push(t);
      arr[i].active = (arr[i].active||0)+1;
      saveRoutes(uid,arr); return t;
    }
    return req(`/routes/${routeId}/teams`,"POST",payload);
  },
  async updateTeam(routeId, teamId, fields){
    if(USE_MOCK){
      const uid = storage.user?.id; const arr = loadRoutes(uid);
      const i = arr.findIndex(x=>x.id==routeId); if(i<0) throw new Error("Ruta no existe");
      const j = (arr[i].teams||[]).findIndex(t=>t.id==teamId); if(j<0) throw new Error("Equipo no existe");
      arr[i].teams[j] = { ...arr[i].teams[j], ...fields };
      saveRoutes(uid,arr); return arr[i].teams[j];
    }
    return req(`/routes/${routeId}/teams/${teamId}`,"PUT",fields);
  },
  async deleteTeam(routeId, teamId){
    if(USE_MOCK){
      const uid = storage.user?.id; const arr = loadRoutes(uid);
      const i = arr.findIndex(x=>x.id==routeId); if(i<0) throw new Error("Ruta no existe");
      const team = (arr[i].teams||[]).find(t=>t.id==teamId);
      arr[i].teams = (arr[i].teams||[]).filter(t=>t.id!=teamId);
      if(team){
        if(team.active){ arr[i].active = Math.max(0,(arr[i].active||0)-1); }
        else { arr[i].inactive = Math.max(0,(arr[i].inactive||0)-1); }
      }
      saveRoutes(uid,arr); return {ok:true};
    }
    return req(`/routes/${routeId}/teams/${teamId}`,"DELETE");
  },
  async toggleTeam(routeId, teamId, isActive){
    if(USE_MOCK){
      const uid = storage.user?.id; const arr = loadRoutes(uid);
      const i = arr.findIndex(x=>x.id==routeId); if(i<0) throw new Error("Ruta no existe");
      const j = arr[i].teams.findIndex(t=>t.id==teamId); if(j<0) throw new Error("Equipo no existe");
      const prev = !!arr[i].teams[j].active;
      arr[i].teams[j].active = !!isActive;
      if(prev!==isActive){
        if(isActive){ arr[i].active++; arr[i].inactive=Math.max(0,(arr[i].inactive||0)-1); }
        else { arr[i].inactive=(arr[i].inactive||0)+1; arr[i].active=Math.max(0,arr[i].active-1); }
      }
      saveRoutes(uid,arr); return arr[i].teams[j];
    }
  }
};
