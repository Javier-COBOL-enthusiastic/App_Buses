param([int]$Port=8080,[string]$Default="Views/login.html")
$Root=(Get-Location).Path
$H=[System.Net.HttpListener]::new()
$H.Prefixes.Add("http://localhost:$Port/"); $H.Start()
Write-Host "Sirviendo $Root en http://localhost:$Port/  (Ctrl+C para detener)"
function Mime($p){ switch(([IO.Path]::GetExtension($p)).ToLower()){
  '.html'{'text/html'} '.css'{'text/css'} '.js'{'application/javascript'}
  '.svg'{'image/svg+xml'} '.json'{'application/json'} '.png'{'image/png'}
  '.jpg'{'image/jpeg'} '.jpeg'{'image/jpeg'} '.webp'{'image/webp'} default{'application/octet-stream'} } }
try{
  while($true){
    $ar=$H.BeginGetContext($null,$null); while(-not $ar.AsyncWaitHandle.WaitOne(50)){}; $ctx=$H.EndGetContext($ar)
    $rel=[Uri]::UnescapeDataString($ctx.Request.Url.AbsolutePath.TrimStart('/')); if([string]::IsNullOrWhiteSpace($rel)){$rel=$Default}
    if($rel -eq 'favicon.ico'){ $ctx.Response.StatusCode=204; $ctx.Response.Close(); continue }
    $rel=$rel -replace '[\r\n\t]',''; if($rel -match '[<>"]'){ $ctx.Response.StatusCode=400; $ctx.Response.Close(); continue }
    $path=Join-Path $Root $rel
    if((Test-Path $path) -and -not (Get-Item $path).PSIsContainer){
      $b=[IO.File]::ReadAllBytes($path); $ctx.Response.ContentType=(Mime $path); $ctx.Response.OutputStream.Write($b,0,$b.Length)
    } else { $ctx.Response.StatusCode=404; $b=[Text.Encoding]::UTF8.GetBytes('404 Not Found'); $ctx.Response.OutputStream.Write($b,0,$b.Length) }
    $ctx.Response.Close()
  }
} finally { $H.Stop(); $H.Close() }
