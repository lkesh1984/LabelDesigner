window.themeManager = {
    setTheme: function (theme)
    {
        document.documentElement.setAttribute("data-theme", theme);
        localStorage.setItem("theme", theme);
    },
    getTheme: function ()
    {
        return localStorage.getItem("theme") || "light";
    },
    loadTheme: function ()
    {
        var theme = localStorage.getItem("theme") || "light";
        document.documentElement.setAttribute("data-theme", theme);
    }
};