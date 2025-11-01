const map = L.map('map').setView([13.69294, -89.21819], 8);

var num = 1;
var ruta = null;
bt1 = document.getElementById("aumentar")
bt2 = document.getElementById("disminuir")

bt1.addEventListener('click', () => {
  num = ++num > 3 ? 1 : num;
  if(ruta != null){
    ruta.remove();
  }
  cargar_ruta(num);
});

bt2.addEventListener('click', () => {
  num = --num < 1 ? 3 : num;  
  if(ruta != null){
    ruta.remove();
  }
  cargar_ruta(num);
});

function cargar_ruta(valor){
  let points = [];  
  let ret = null;
  url = `http://localhost:5000/coords/${valor}`
  fetch(url).then(res =>{
    if(res.ok){
      res.json().then( arr =>{
        for(let i = 0; i < arr["lat"].length; i++){
          let lat = arr["lat"][i];
          let lon = arr["lon"][i];
          if (!isNaN(lat) && !isNaN(lon)) {
              points.push(L.latLng(lat, lon));
          }
        }  
      mostrar_linea(points);                
      }          
      );
    }
    }      
  )    
}

function mostrar_linea(points){
  ruta = L.polyline(points, {color:"red"})
  ruta.addTo(map);  
}

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
  maxZoom: 19,
  attribution: '© OpenStreetMap contributors'
}).addTo(map);
