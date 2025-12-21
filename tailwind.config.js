/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Views/**/*.cshtml",
        "./Areas/**/*.cshtml",
        "./wwwroot/js/**/*.js"
    ],
    theme: {
        extend: {
            colors: {
                navy: {
                    900: '#020617',
                    800: '#0f172a',
                }
            }
},
    },
    plugins: [],
}
