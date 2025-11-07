export const storage = {
  get token(){ return localStorage.getItem("bt_token") || ""; },
  set token(v){ localStorage.setItem("bt_token", v); },
  get user(){ try{ return JSON.parse(localStorage.getItem("bt_user")||"null"); }catch{ return null } },
  set user(u){ localStorage.setItem("bt_user", JSON.stringify(u)); },
  logout(){ localStorage.removeItem("bt_token"); localStorage.removeItem("bt_user"); }
};
