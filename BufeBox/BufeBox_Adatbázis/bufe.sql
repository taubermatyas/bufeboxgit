-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Ápr 09. 10:36
-- Kiszolgáló verziója: 10.4.28-MariaDB
-- PHP verzió: 8.1.17

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";
SET FOREIGN_KEY_CHECKS = 0; -- Idegen kulcs ellenőrzés kikapcsolása a biztonságos létrehozás érdekében

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `bufe`
--
DROP DATABASE IF EXISTS `bufe`;
CREATE DATABASE IF NOT EXISTS `bufe` DEFAULT CHARACTER SET utf8 COLLATE utf8_hungarian_ci;
USE `bufe`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `dolgozo`
--

CREATE TABLE `dolgozo` (
  `felnev` varchar(255) NOT NULL,
  `nev` varchar(255) NOT NULL,
  `jelszo` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `dolgozo`
--

INSERT INTO `dolgozo` (`felnev`, `nev`, `jelszo`) VALUES
('dolgozo1', 'Dolgozo1', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `kategoria`
--

CREATE TABLE `kategoria` (
  `kid` int(11) NOT NULL,
  `knev` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `kategoria`
--

INSERT INTO `kategoria` (`kid`, `knev`) VALUES
(1, 'Friss Áruk'),
(2, 'Helyben készült termékek'),
(3, 'Italok'),
(4, 'Forró italok'),
(5, 'Tejtermékek'),
(6, 'Csomagolt termékek');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `kosar`
--

CREATE TABLE `kosar` (
  `kkod` int(11) NOT NULL,
  `idopont` datetime NOT NULL,
  `email` varchar(255) NOT NULL,
  `felnev` varchar(255) NOT NULL,
  `megjegyzes` varchar(255) NOT NULL,
  `elkeszult` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `kosar`
--

INSERT INTO `kosar` (`kkod`, `idopont`, `email`, `felnev`, `megjegyzes`, `elkeszult`) VALUES
(1, '2025-03-21 00:00:00', 'vasarlo1@gmail.com', 'dolgozo1', 'Finom legyen', 0),
(2, '2025-04-07 08:40:11', 'vasarlo2@gmail.com', 'dolgozo1', 'jo lesz', 0),
(3, '2025-04-07 11:27:32', 'vasarlo3@gmail.com', 'dolgozo1', 'elegem van', 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `kosarba`
--

CREATE TABLE `kosarba` (
  `kkod` int(11) NOT NULL,
  `tid` int(11) NOT NULL,
  `tmenny` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `kosarba`
--

INSERT INTO `kosarba` (`kkod`, `tid`, `tmenny`) VALUES
(1, 2, 3),
(1, 3, 2),
(2, 5, 3),
(2, 7, 2),
(3, 8, 2);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `szamla`
--

CREATE TABLE `szamla` (
  `szkod` int(11) NOT NULL,
  `osszeg` int(11) NOT NULL,
  `email` varchar(255) NOT NULL,
  `termekek` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `termek`
--

CREATE TABLE `termek` (
  `tid` int(11) NOT NULL,
  `tnev` varchar(255) NOT NULL,
  `mennyiseg` int(11) NOT NULL,
  `kiszereles` varchar(255) NOT NULL,
  `ar` int(11) NOT NULL,
  `afa` int(11) NOT NULL,
  `kid` int(11) NOT NULL,
  `kepUrl` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `termek`
--

INSERT INTO `termek` (`tid`, `tnev`, `mennyiseg`, `kiszereles`, `ar`, `afa`, `kid`, `kepUrl`) VALUES
(1, 'Sajtos pogi', 20, 'db', 100, 27, 1, 'https://pogacsarendeles.hu/wp-content/uploads/2018/02/extrasajtosf.jpg'),
(2, 'Pizzás mini', 20, 'db', 100, 27, 1, 'https://pogacsarendeles.hu/wp-content/uploads/2018/02/pizzas.jpg'),
(3, 'Barackos mini', 20, 'db', 100, 27, 1, 'https://pogacsarendeles.hu/wp-content/uploads/2018/02/barackosfornetti.jpg'),
(4, 'Mini grissini', 20, 'db', 100, 27, 1, 'https://www.fornetti.hu/files/a/b/sajtos_grissini_400x400_c.jpg'),
(5, 'Kakaós roll mini', 20, 'db', 160, 27, 1, 'httphttps://www.fornetti.hu/files/8/d/csokistekercs_400x400_c.jpg'),
(6, 'Sajtos rúd', 20, 'db', 450, 27, 1, 'https://www.fornetti.hu/files/c/5/extra_sajtos_rud.jpg'),
(7, 'Kakaós csiga', 20, 'db', 450, 27, 1, 'https://www.fornetti.hu/files/1/4/xxl_kakaos.jpg'),
(8, 'Dupla csokis maxi', 20, 'db', 450, 27, 1, 'https://www.fornetti.hu/files/4/7/xxl_csokis.jpg'),
(9, 'Fánk', 20, 'db', 400, 27, 1, 'https://www.fornetti.hu/files/a/7/malnas_fank_400x400_c.jpg'),
(10, 'Virslis maxi', 20, 'db', 450, 27, 1, 'https://www.fornetti.hu/files/f/4/xxl_hotdog.jpg'),
(11, 'Muffin', 20, 'db', 500, 27, 1, 'https://www.dine4fit.hu/file/image/foodstuff/44c237cb767cfe43/1197754f93614a44909e9ba8
ce5fda42'),
(12, 'Pizza szelet', 20, 'db', 700, 27, 1, 'https://www.fornetti.hu/files/d/5/kukoricas_pizza.jpg'),
(13, 'Goofree', 20, 'db', 400, 27, 1, 'https://www.kamraellato.hu/img/45130/1001653/5999560730036.webp?time=1684137898'),
(14, 'Nachos', 20, 'db', 600, 27, 1, 'https://www.femcafe.hu/sites/default/files/styles/fb_landscape/public/images2017/cikkek/nac
hos_123.jpg'),
(15, 'Óriás perec', 20, 'db', 450, 27, 1, 'https://static.groby.hu/media/d2b/5af/conv/56436_se1_d9b5e_back-full.png'),
(16, 'Szendvics', 20, 'db', 450, 27, 2, 'https://encrypted-
tbn0.gstatic.com/images?q=tbn:ANd9GcQN_TZKUjHqpWTOXsSupB4kKtn11vZj_lGXnA&amp;
s'),
(17, 'Rántott húsos szendvics', 20, 'db', 800, 27, 2, 'https://encrypted-
tbn0.gstatic.com/images?q=tbn:ANd9GcQlRe2ZeJnua58PL6fzdUVgZ5GbG8dKyMeHlw&amp;s'),
(18, 'Melegszendvics', 20, 'db', 550, 27, 2, 'https://static.streetkitchen.hu/live/uploads/2023/01/zsiros-kenyer-
melegszendvics-1-1618x855.jpg'),
(19, 'Hot-dog', 20, 'db', 550, 27, 2, 'https://media-cdn2.greatbritishchefs.com/media/q45jfdf3/gbc_jamesknappett-
5.whqc_768x512q80.jpg'),
(20, 'Hamburger', 20, 'db', 800, 27, 2, 'https://listonic.com/phimageproxy/listonic/products/hamburgers.webp'),
(21, 'Gyümölcssaaláta', 20, 'doboz', 500, 27, 2, 'https://noihirek.hu/pictures/gasztro/gyumolcssalata_1.jpg'),
(22, 'Majonézes Kukoricasaláta', 20, 'doboz', 400, 27, 2, 'https://kep.index.hu/1/0/5669/56691/566918/56691873_4191807_9b38f51427889d10e7ec5c8
e985b2501_wm.jpg'),
(23, 'Top-joy', 20, 'üveg', 350, 27, 3, 'https://encrypted-
tbn0.gstatic.com/images?q=tbn:ANd9GcRBk3VwTZBfkdO3e_Fk6WXkRwxGBw6TOLF4J
Q&amp;s'),
(24, 'Xixo', 20, 'doboz', 300, 27, 3, 'https://static.groby.hu/media/922/76d/conv/XIXO-black-cherry-%281%29-full.png'),
(25, 'Üditő', 20, 'doboz', 550, 27, 3, 'https://i0.wp.com/sandwich.hu/wp-content/uploads/2023/08/Cola_500ml.webp'),
(26, 'Powerade', 20, 'üveg', 600, 27, 3, 'https://www.csokibarat.hu/img/termekek/3010/small/_Powerade_Sports_Mountain_Blast_500
ml__0.jpg'),
(27, 'Szörp', 20, 'üveg', 400, 27, 3, 'https://kep.index.hu/1/0/3312/33123/331231/33123100_f58248d178e7f29d5ae174db5c452e2
9_wm.jpg'),
(28, 'Naturaqua', 20, 'üveg', 300, 27, 3, 'https://firsthand.hu/25778-medium_default/naturaqua-szensavmentes-termeszetes-
asvanyviz-05l-drs.jpg'),
(29, 'Forró csoki', 20, 'pohár', 400, 27, 4, 'https://testszerviz.hu/evcms_medias/upload/images/hot_chocolate_1058197_1280%281%29.j
pg'),
(30, 'Gyümölcs tea', 20, 'pohár', 300, 27, 4, 'https://st.depositphotos.com/1177973/3393/i/450/depositphotos_33931713-
stock-photo-delicious-strawberry-tea-on-table.jpg'),
(31, 'Presszó kávé', 20, 'pohár', 350, 27, 4, 'https://www.caffeservice.hu/wp-content/uploads/2021/01/lavazza-espresso-
kave.jpg'),
(32, 'Hosszú kávé', 20, 'pohár', 350, 27, 4, 'https://13d8a4141b.cbaul-
cdnwnd.com/5811553b4aadc040e82b74e5ca44f91b/200000017-c256bc3528/image-crop-
29lx52.jpeg?ph=13d8a4141b'),
(33, 'Kávé tejjel', 20, 'pohár', 400, 27, 4, 'https://bestbarista.hu/wp-content/uploads/2023/08/cortado.jpg'),
(34, 'Cappuccino', 20, 'pohár', 400, 27, 4, 'https://www.thespruceeats.com/thmb/oUxhx54zsjVWfPlrgedJU0MZ-
y0=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/how-to-make-cappuccinos-
766116-hero-01-a754d567739b4ee0b209305138ecb996.jpg'),
(35, 'Csokis cappuccino', 20, 'pohár', 400, 27, 4, 'https://claracaffe.shoprenter.hu/custom/claracaffe/image/cache/w400h267q80np1/cappuccino
%20h%C3%A1zilag.jpg'),
(36, 'Puding', 20, 'tál', 250, 27, 5, 'https://goldmilkkft.cdn.shoprenter.hu/custom/goldmilkkft/image/data/product/1080100.jpg.we
bp?lastmod=1720596902.1665741935'),
(37, 'Tejszelet', 20, 'db', 250, 27, 5, 'https://shop.hosso.hu/custom/hossoabc1/image/cache/w1910h1000/PRD/4008400190501.jpg.
webp?lastmod=1720582214.1729365623'),
(38, 'Müllermilch', 20, 'db', 600, 27, 5, 'https://encrypted-
tbn0.gstatic.com/images?q=tbn:ANd9GcSfb6msOK5Vnt9IQQ1mmoTGEAIU5dNYvN-
7eA&amp;s'),
(39, 'Traccs parti kukoricapehely', 20, 'db', 350, 27, 6, 'https://encrypted-
tbn0.gstatic.com/images?q=tbn:ANd9GcSP5Oc05E_7MgpLyyVK2lIr4SexPJCo_Z9Vxg&amp;s'),
(40, 'Rice up chips', 20, 'db', 400, 27, 6, 'https://otthondekorshop.cdn.shoprenter.hu/custom/otthondekorshop/image/data/Rice%20UP%
21/Rice%20UP%21%20barbecue%20%C3%ADzes%C3%ADt%C3%A9s%C5%B1%20barn
a%20rizs%20chips%2060%20g%20-
%20RUBIBRC60.png.webp?lastmod=1720594443.1692191342'),
(41, 'Tuc pizzás', 20, 'db', 500, 27, 6, 'https://media.prezzemoloevitale.it/media/catalog/product/cache/bd5be9fd6288a362f2aa2b4b9
d4f6e10/n/u/nuovo_progetto_-_2023-08-11t121601.240.jpg'),
(42, '7days croissant', 20, 'db', 350, 27, 6, 'https://sixi2000.cdn.shoprenter.hu/custom/sixi2000/image/data/product/cHJvZHVjdHM9Mjg
5ODI.jpg.webp?lastmod=1720606412.1705908054'),
(43, 'Orbit rágó', 20, 'db', 350, 27, 6, 'https://www.marazplast.hu/img/27548/50173204/500x500/50173204.webp?time=171818738
8'),
(44, 'Milka tábla csoki', 20, 'db', 600, 27, 6, 'https://www.koffeinzona.hu/img/55341/1003312/500x500,r/7622400005190.jpg?time=16999
74929'),
(45, 'Milky way', 20, 'db', 350, 27, 6, 'https://www.britishfoodshop.com/cdn/shop/products/106154.jpg?v=1611389144'),
(46, 'Twix', 20, 'db', 400, 27, 6, 'https://pepperyspot.com/cdn/shop/files/twix-50.jpg?v=1721409908&amp;width=1946'),
(47, 'Cerbona szelet', 20, 'db', 300, 27, 6, 'https://www.naturteka.hu/image/403043495.jpg'),
(48, 'Brownie', 20, 'db', 400, 27, 6, 'https://listonic.com/phimageproxy/listonic/products/brownie.webp'),
(49, 'Szőlőcukor', 20, 'db', 250, 27, 6, 'https://www.napibio.hu/image/cache/data/termek03/szolocukor-tabletta-narancs-
m-75g-220x220.jpg.webp'),
(50, 'Papírzsebkendő', 20, 'csomag', 150, 27, 6, 'https://centrumpapir.hu/wp-content/uploads/sindy_10_es_2336.jpg');


-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `vasarlo`
--

CREATE TABLE `vasarlo` (
  `email` varchar(255) NOT NULL,
  `nev` varchar(255) NOT NULL,
  `jelszo` blob NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `vasarlo`
--

INSERT INTO `vasarlo` (`email`, `nev`, `jelszo`) VALUES
('vasarlo1@gmail.com', 'Vasarlo1', 0x31323334),
('vasarlo2@gmail.com', 'vasarlo2', 0x31323334),
('vasarlo3@gmail.com', 'Madron', 0x31323334);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `dolgozo`
--
ALTER TABLE `dolgozo`
  ADD PRIMARY KEY (`felnev`);

--
-- A tábla indexei `kategoria`
--
ALTER TABLE `kategoria`
  ADD PRIMARY KEY (`kid`);

--
-- A tábla indexei `kosar`
--
ALTER TABLE `kosar`
  ADD PRIMARY KEY (`kkod`),
  ADD KEY `felhasz` (`email`),
  ADD KEY `felnev` (`felnev`);

--
-- A tábla indexei `kosarba`
--
ALTER TABLE `kosarba`
  ADD PRIMARY KEY (`kkod`,`tid`),
  ADD KEY `tid` (`tid`);

--
-- A tábla indexei `szamla`
--
ALTER TABLE `szamla`
  ADD PRIMARY KEY (`szkod`),
  ADD KEY `email` (`email`);

--
-- A tábla indexei `termek`
--
ALTER TABLE `termek`
  ADD PRIMARY KEY (`tid`),
  ADD KEY `kid` (`kid`);

--
-- A tábla indexei `vasarlo`
--
ALTER TABLE `vasarlo`
  ADD PRIMARY KEY (`email`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `kategoria`
--
ALTER TABLE `kategoria`
  MODIFY `kid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT a táblához `kosar`
--
ALTER TABLE `kosar`
  MODIFY `kkod` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `szamla`
--
ALTER TABLE `szamla`
  MODIFY `szkod` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `termek`
--
ALTER TABLE `termek`
  MODIFY `tid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `kosar`
--
ALTER TABLE `kosar`
  ADD CONSTRAINT `kosar_ibfk_1` FOREIGN KEY (`email`) REFERENCES `vasarlo` (`email`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `kosar_ibfk_3` FOREIGN KEY (`felnev`) REFERENCES `dolgozo` (`felnev`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Megkötések a táblához `kosarba`
--
ALTER TABLE `kosarba`
  ADD CONSTRAINT `kosarba_ibfk_1` FOREIGN KEY (`tid`) REFERENCES `termek` (`tid`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `kosarba_ibfk_2` FOREIGN KEY (`kkod`) REFERENCES `kosar` (`kkod`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Megkötések a táblához `szamla`
--
ALTER TABLE `szamla`
  ADD CONSTRAINT `szamla_ibfk_1` FOREIGN KEY (`email`) REFERENCES `vasarlo` (`email`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Megkötések a táblához `termek`
--
ALTER TABLE `termek`
  ADD CONSTRAINT `termek_ibfk_1` FOREIGN KEY (`kid`) REFERENCES `kategoria` (`kid`) ON DELETE CASCADE ON UPDATE CASCADE;

SET FOREIGN_KEY_CHECKS = 1; -- Idegen kulcs ellenőrzés visszakapcsolása

COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;