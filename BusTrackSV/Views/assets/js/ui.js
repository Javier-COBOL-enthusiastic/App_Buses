export const $ = (sel,root=document)=>root.querySelector(sel);
export const $$ = (sel,root=document)=>[...root.querySelectorAll(sel)];

export function toast(msg,type='ok'){
  let wrap = $('.toast-wrap'); if(!wrap){ wrap = document.createElement('div'); wrap.className='toast-wrap'; document.body.appendChild(wrap); }
  const el = document.createElement('div'); el.className = 'toast ' + (type||'');
  el.textContent = msg; wrap.appendChild(el);
  setTimeout(() => {el.classList.add('hide');setTimeout(() => el.remove(), 300);}, 720);
}

export function openModal(id){ const m=$("#"+id); if(m){ m.classList.add("open"); document.documentElement.classList.add("has-modal"); } }
export function closeModal(id){ const m=$("#"+id); if(m){ m.classList.remove("open"); } if(!document.querySelector(".modal.open")){ document.documentElement.classList.remove("has-modal"); } }

export function setThemeToggle(btn){
  if(!btn) return;
  const apply = t => { document.documentElement.setAttribute('data-theme', t); localStorage.setItem('bt_theme', t); btn.innerHTML = t==='dark'? moonSvg : sunSvg; };
  const saved = localStorage.getItem('bt_theme') || 'light';
  apply(saved);
  btn.addEventListener('click',()=> apply( (document.documentElement.getAttribute('data-theme')==='dark') ? 'light':'dark'));
}
const sunSvg = `<svg width="22" height="22" viewBox="0 0 24 24" fill="none"><circle cx="12" cy="12" r="4" stroke="currentColor" stroke-width="2"/><path d="M12 2v2M12 20v2M4 12H2M22 12h-2M5 5l1.5 1.5M17.5 17.5L19 19M19 5l-1.5 1.5M5 19l1.5-1.5" stroke="currentColor" stroke-width="2" stroke-linecap="round"/></svg>`;
const moonSvg = `<svg width="22" height="22" viewBox="0 0 24 24" fill="none"><path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" stroke="currentColor" stroke-width="2" fill="none"/></svg>`;

