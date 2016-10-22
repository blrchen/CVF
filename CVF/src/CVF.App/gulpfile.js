/// <binding Clean='clean' />
'use strict';

var gulp = require('gulp');

var paths = {
    webroot: './wwwroot/',
    module: './node_modules/',
    lib: './wwwroot/lib/'
};

paths.js = paths.webroot + 'js/**/*.js';
paths.minJs = paths.webroot + 'js/**/*.min.js';
paths.css = paths.webroot + 'css/**/*.css';
paths.minCss = paths.webroot + 'css/**/*.min.css';
paths.concatJsDest = paths.webroot + 'js/site.min.js';
paths.concatCssDest = paths.webroot + 'css/site.min.css';

gulp.task('copy', function () {
    var npm = {
        'angular': 'angular/angular*.js',
        'angular-ui-bootstrap': 'angular-ui-bootstrap/dist/ui-bootstrap*.{js,css}',
        'angular-chart.js': 'angular-chart.js/dist/*.js',
        'angular-route': 'angular-route/angular-route.js',
        'bootstrap': 'bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,eot}',
        'chart.js': 'chart.js/dist/*Chart.js',
        'font-awesome': 'font-awesome/**/*.{js,map,css,ttf,svg,woff,eot}',
        'jquery': 'jquery/dist/jquery*.{js,map}',
        'metisMenu': 'metisMenu/dist/*.{js,css}'
    }

    for (var destinationDir in npm) {
        gulp.src(paths.module + npm[destinationDir])
          .pipe(gulp.dest(paths.lib + destinationDir));
    }
});
