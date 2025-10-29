const map = L.map('map').setView([13.69294, -89.21819], 8);


fetch("http://localhost:5000/dir1").then(res =>{
  if(res.ok){
    res.json().then(e => console.log(e));
  }
  }  
)







// document.getElementById("data").addEventListener('change', function(e) {
//     const file = e.target.files[0];
//     const reader = new FileReader();    

//     reader.onload = function(file) {
//     const d = file.target.result;
//     const f = d.trim().split('\n'); 
//     const points = [];

//     for (let i = 1; i < f.length; i++) { 
//         const cols = f[i].split(','); 
//         const lat = parseFloat(cols[1]);  
//         const lon = parseFloat(cols[2]);  
//         if (!isNaN(lat) && !isNaN(lon)) {
//             points.push(L.latLng(lat, lon));
//         }
//     }
//     L.polyline(points, {color:"red"}).addTo(map);
//   }

//   reader.readAsText(file);
// })


L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
  maxZoom: 19,
  attribution: '© OpenStreetMap contributors'
}).addTo(map);