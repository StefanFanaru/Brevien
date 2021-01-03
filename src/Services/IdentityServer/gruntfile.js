const sass = require('node-sass')

module.exports = function (grunt) {

    require('load-grunt-tasks')(grunt)

    grunt.initConfig({

        paths: {
            resources: 'Identity.API/wwwroot/sass', // source files (scss)
            assets: 'Identity.API/wwwroot/css'       // compiled files (css)
        },

        // Sass
        sass: {
            dev: {
                files: {
                    '<%= paths.assets %>/main.css': '<%= paths.resources %>/main.scss'
                },
                options: {
                    implementation: sass,
                    style: 'expanded'
                }
            },
            prod: {
                files: {
                    '<%= paths.assets %>/main.css': '<%= paths.resources %>/main.scss'
                },
                options: {
                    implementation: sass,
                    style: 'compressed', // This option minimizes the CSS
                    sourcemap: 'none'
                }
            }
        },
        watch: {
            options: {
                livereload: true
            },
            css: {
                files: ['Identity.API/wwwroot/sass/*.scss'],
                tasks: ['sass:dev']
            }
        }
    })

    grunt.registerTask('dev', ['sass:dev'])
    grunt.registerTask('prod', ['sass:prod'])
}
