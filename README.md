# CVF
A prototype code for evaluating using Angular & ASP.NET Core to build web app with isolated plugin model.

# Live demo
http://cvf.azurewebsites.net/

# Plugin development
See wwwroot/plugins/demo for demo plugin sample.
- Manifest.json is plugin description file. CVF will read this file to compose plugin UI
- Api can be hosted any where, but need reference CVF.Contract  
- Use angular data-binding