var sass = require("gulp-dart-sass");
var async = require("async");
var rename = require("gulp-rename");
var cleanCSS = require('gulp-clean-css');
var uglify = require('gulp-uglify');
var gulp = require('gulp');

const buildSass = () =>
    gulp.src("Styles/*.scss")
        .pipe(sass().on("error", sass.logError))
        .pipe(cleanCSS())
        .pipe(gulp.dest("wwwroot/css"));

gulp.task("build-fe", () => {
    return async.series([
        (next) => buildSass().on("end", next)
    ])
}); 