function Invoices({toggleSidebar}) {
    return (
        <div>
        <head>
        <title>Home</title>
         </head>
        <section class="welcome">
            <button id="menu-toggle" class="menu-toggle" onClick={toggleSidebar}>☰</button>
            <h1>Welcome to Your Dashboard</h1>
        </section>

        <section class="dashboard-grid">
            <article class="dashboard-column">
                <div class="widget">
                    <h3>Widget 1</h3>
                    <div class="popup" onclick="myFunction()">Click me!
                        <span class="popuptext" id="myPopup">Popup text...</span>
                    </div>
                </div>
            </article>
            <article class="dashboard-column">
                <div class="widget">
                    <h3>Widget 2</h3>
                    <p>Content goes here...</p>
                </div>
            </article>
            <article class="dashboard-column">
                <div class="widget">
                    <h3>Widget 3</h3>
                    <p>Content goes here...</p>
                </div>
            </article>
            <article class="dashboard-column">
                <div class="widget">
                    <h3>Widget 4</h3>
                    <p>Content goes here...</p>
                </div>
            </article>
            <article class="dashboard-column">
                <div class="widget">
                    <h3>Widget 5</h3>
                    <p>Content goes here...</p>
                </div>
            </article>
        </section>
        </div>
    );
}

export default Invoices;