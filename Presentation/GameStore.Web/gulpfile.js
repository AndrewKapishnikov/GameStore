/// <binding BeforeBuild='clean:js, min:js' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    uglify = require("gulp-uglify");

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/site.js";
paths.concatJsDest = paths.webroot + "bundle/js/site.min.js";


gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

//gulp.task("clean", gulp.series(["clean:js"]));

gulp.task("min:js", function () {
    return gulp.src([paths.js], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

//gulp.task("min", gulp.series(["min:js"]));

