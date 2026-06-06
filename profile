<!doctype html>
<html lang="ru">
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Профиль</title>
    <link rel="stylesheet" href=""> <!--Связываем с классом href="" CSS-->
</head>
<body data-page="profile">

<header class="bar">
    <a class="brand" href="profile.html"><span class="logoMark">Аукцион</span></a>
    <nav class="navigat">
        <a href="profile.html" data-page-link="profile">Профиль</a>
        <a href="notifications.html" data-page-link="notifications">Уведомления</a>
        <a href="settings.html" data-page-link="settings">Настройки</a>
    </nav>
    <div class="search"><input type="search" placeholder="Найти"></div>
    <a href="profile.html" class="userRound" data-user-initials>Профиль</a>
</header>

<main class="main">
    <h1>Профиль</h1>
    <div class="profileGrid">
        
<aside class="sideMenu">
    <a href="profile.html" data-page-link="profile">Профиль</a>
    <a href="profile.html#bids">Мои ставки</a>
    <a href="notifications.html" data-page-link="notifications">Уведомления</a>
    <a href="settings.html" data-page-link="settings">Настройки</a>
</aside>

        <section class="contentCol">
            <div class="profileTop">
                <div class="personCard card">
                    <div class="bigAvatar"><span data-user-initials>Профиль</span></div>
                    <div>
                        <h2 data-user-name></h2> <!-- Данные подтягиваются при входе в аккаунт-->
                        <p class="muted" data-user-email></p> <!-- Данные подтягиваются при входе в аккаунт-->
                        <p class="muted"></p> <!-- Данные подтягиваются при входе в аккаунт-->
                        <a href="settings.html" class="btn">Редактировать профиль</a> <!-- БД-->
                    </div>
                </div> <!-- Пока заглушки потом реальзуем функцию чтобы подтягивалось из БД-->
                <!-- Пока тут ничего нет, заглушка-->
            </div>

            <div class="Cols">
                <div class="card panelPad">
                    <h2>Уведомления</h2>
                    <div id="smallNotes"></div>
                    <p><a href="notifications.html" class="muted">Все уведомления</a></p>
                </div>
                <div class="card balanceBox"><!-- Пока заглушки потом реальзуем функцию чтобы подтягивалось из БД-->
                    <h2>Баланс</h2>
                    <!--пока заглушка потом свяжим -->
                    <button class="btn">Пополнить</button>
                </div>
            </div>

            <div class="card panelPad" id="bids">
                <h2>Отслеживаемые лоты</h2><!-- Пока заглушки потом реальзуем функцию чтобы подтягивалось из БД-->
            </div>

            <div class="card panelPad">
                <h2>Мои последние ставки</h2>
                <table class="tableSimple"><!-- Пока заглушки потом реальзуем функцию чтобы подтягивалось из БД-->
                    
                </table>
                <div class="saveLine"><button class="btn danger" id="logoutBtn">Выйти из аккаунта</button></div>
            </div>
        </section>
    </div>
</main>
<script src=""></script> <!--JS-->
</body>
</html>
