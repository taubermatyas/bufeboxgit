// Oldal betöltésekor futó inicializáló függvények
document.addEventListener('DOMContentLoaded', async () => {
    await menuKategoriakBetoltese();
    felhasznaloEllenorzes();
    etelekMegjelenitese();
    galeriaInditasa();
    idopontFrissites();
    setInterval(idopontFrissites, 1000);
    esemenyKezelokBeallitasa();
    hamburgerMenuInicializalas();

    const betolto = document.getElementById('betolto');
    if (betolto) {
        betolto.style.opacity = '0';
        setTimeout(() => betolto.remove(), 500);
    }
});

// Görgetéskor az étel kártyák animált megjelenítése
window.addEventListener('scroll', () => {
    const kartyak = document.querySelectorAll('.etel-kartya');
    if (!kartyak.length) return;

    const sorok = [];
    let aktualisSor = [];
    let utolsoTop = kartyak[0].getBoundingClientRect().top;

    kartyak.forEach(kartya => {
        const kartyaTop = kartya.getBoundingClientRect().top;
        if (Math.abs(kartyaTop - utolsoTop) > 10) {
            if (aktualisSor.length) sorok.push(aktualisSor);
            aktualisSor = [];
        }
        aktualisSor.push(kartya);
        utolsoTop = kartyaTop;
    });
    if (aktualisSor.length) sorok.push(aktualisSor);

    sorok.forEach((sor, sorIndex) => {
        const sorTop = sor[0].getBoundingClientRect().top;
        const kepernyoMagassag = window.innerHeight;

        if (sorTop < kepernyoMagassag - 50) {
            sor.forEach((kartya, kartyaIndex) => {
                if (!kartya.classList.contains('lathato')) {
                    setTimeout(() => {
                        kartya.classList.add('lathato');
                    }, kartyaIndex * 100);
                }
            });
        }
    });
});

// Globális változók
let kosarTartalom = [];
let galeriaIndex = 0;
let aktivKategoria = 'osszes';
const API_URL = 'https://localhost:44398/api';

// Értesítés megjelenítése
const ertesitesMegjelenites = (uzenet, tipus = 'sikeres') => {
    const ertesites = document.createElement('div');
    ertesites.className = `ertesites ${tipus}`;
    ertesites.innerHTML = `
        <div class="ikon">${tipus === 'sikeres' ? '✓' : '✗'}</div>
        <h3>${tipus === 'sikeres' ? 'Siker!' : 'Hiba!'}</h3>
        <p>${uzenet}</p>
        <button onclick="this.parentElement.classList.add('rejtett'); setTimeout(() => this.parentElement.remove(), 500)">Bezár</button>
        <div class="folyamatjelzo"></div>
    `;
    document.body.appendChild(ertesites);
    setTimeout(() => ertesites.classList.add('lathato'), 10);
    setTimeout(() => {
        ertesites.classList.add('rejtett');
        setTimeout(() => ertesites.remove(), 500);
    }, 5000);
};

// Gyors DOM elem lekérése
const elem = id => document.getElementById(id);

// Interakciók tiltása bejelentkezés nélkül
const interakciokTiltasa = () => {
    document.querySelectorAll('.kosarba, .kosar, .menu button, .etel-kartya, #kereso-mezo').forEach(el => {
        el.disabled = true;
        el.style.pointerEvents = 'none';
        el.style.opacity = '0.5';
    });
};

// Interakciók engedélyezése
const interakciokEngedelyezese = () => {
    document.querySelectorAll('.kosarba, .kosar, .menu button, .etel-kartya, #kereso-mezo').forEach(el => {
        el.disabled = false;
        el.style.pointerEvents = 'auto';
        el.style.opacity = '1';
    });
};

// Felhasználó bejelentkezésének ellenőrzése
const felhasznaloEllenorzes = () => {
    localStorage.getItem('bejelentkezve') === 'true' ? interakciokEngedelyezese() : interakciokTiltasa();
};

// Menü kategóriák betöltése
const menuKategoriakBetoltese = async () => {
    const menu = elem('menu');
    if (!menu) return;

    const keresoKontener = menu.querySelector('.kereso-kontener');
    const kosarGomb = menu.querySelector('.kosar');
    menu.innerHTML = '<p>Betöltés folyamatban...</p>';

    try {
        const controller = new AbortController();
        setTimeout(() => controller.abort(), 5000);
        const response = await fetch(`${API_URL}/Kategoria`, { headers: { 'Accept': 'application/json' }, signal: controller.signal });
        if (!response.ok) throw new Error('Hiba a kategóriák betöltésekor');
        const kategoriak = await response.json();

        menu.innerHTML = '';
        const osszesGomb = document.createElement('button');
        osszesGomb.textContent = 'Minden étel';
        osszesGomb.classList.add('aktiv');
        osszesGomb.addEventListener('click', () => menuSzures('osszes'));
        menu.appendChild(osszesGomb);

        kategoriak.forEach(kategoria => {
            const gomb = document.createElement('button');
            gomb.textContent = kategoria.Knev;
            gomb.addEventListener('click', () => menuSzures(kategoria.Knev.toLowerCase()));
            menu.appendChild(gomb);
        });

        menu.appendChild(keresoKontener);
        menu.appendChild(kosarGomb);
        menuSzures('osszes');
    } catch (hiba) {
        console.error('Kategóriák betöltési hiba:', hiba);
        menu.innerHTML = '<p style="color: red;">Nem sikerült betölteni a kategóriákat.</p>';
        menu.appendChild(keresoKontener);
        menu.appendChild(kosarGomb);
    }
};

// Ételek betöltése és megjelenítése
const etelekMegjelenitese = async () => {
    const etelRacs = elem('etel-racs');
    if (!etelRacs) return;

    try {
        const controller = new AbortController();
        setTimeout(() => controller.abort(), 5000);
        const response = await fetch(`${API_URL}/Termek`, { headers: { 'Accept': 'application/json' }, signal: controller.signal });
        if (!response.ok) throw new Error('Hiba az ételek betöltésekor');
        const termekek = await response.json();

        etelRacs.innerHTML = '';
        termekek.forEach(termek => {
            const etelKartya = document.createElement('div');
            etelKartya.className = 'etel-kartya';
            etelKartya.dataset.kategoria = termek.Kategoria?.Knev?.toLowerCase() || 'ismeretlen';
            const kepUrl = termek.KepUrl?.trim() || 'https://via.placeholder.com/150';
            const elerheto = termek.Mennyiseg > 0;
            etelKartya.innerHTML = `
                <img src="${kepUrl}" alt="${termek.Tnev}">
                <h3>${termek.Tnev}</h3>
                <p>${termek.Mennyiseg} db elérhető</p>
                <div class="kosarba" ${elerheto ? `onclick="kosarbaTesz('${termek.Tnev}', ${termek.Ar}, ${termek.Tid})"` : 'style="pointer-events: none; opacity: 0.5;"'}>
                    🛒<span>${termek.Ar} Ft</span>
                </div>
            `;
            if (!elerheto) {
                etelKartya.style.opacity = '0.7';
                etelKartya.title = 'Jelenleg nem kapható';
            }
            etelRacs.appendChild(etelKartya);
        });

        szuresFrissites();
    } catch (hiba) {
        console.error('Ételek betöltési hiba:', hiba);
        etelRacs.innerHTML = '<p style="color: red;">Nem sikerült betölteni az ételeket.</p>';
    }
};

// Menü szűrése kategória alapján
const menuSzures = kategoria => {
    aktivKategoria = kategoria;
    document.querySelectorAll('.menu button').forEach(gomb => {
        gomb.classList.toggle('aktiv', gomb.textContent.toLowerCase() === kategoria.toLowerCase());
    });
    szuresFrissites();
};

// Szűrők alkalmazása
const szuresFrissites = () => {
    const kereses = elem('kereso-mezo')?.value.toLowerCase().trim() || '';
    document.querySelectorAll('.etel-kartya').forEach(kartya => {
        const kategoria = kartya.dataset.kategoria || 'ismeretlen';
        const nev = kartya.querySelector('h3')?.textContent.toLowerCase() || '';
        const kategoriaEgyezik = aktivKategoria === 'osszes' || kategoria.toLowerCase() === aktivKategoria.toLowerCase();
        const keresesEgyezik = nev.includes(kereses);

        kartya.style.display = kategoriaEgyezik && keresesEgyezik ? 'block' : 'none';
        kartya.classList.toggle('lathato', kategoriaEgyezik && keresesEgyezik && kartya.getBoundingClientRect().top < window.innerHeight - 100);
    });
};

// Étel kosárba helyezése
const kosarbaTesz = async (nev, ar, tid) => {
    if (localStorage.getItem('bejelentkezve') !== 'true') {
        ertesitesMegjelenites('Kérjük, jelentkezzen be a kosár használatához!', 'hiba');
        profilMegnyitas(true);
        return;
    }

    try {
        const response = await fetch(`${API_URL}/Termek/${tid}`, { headers: { 'Accept': 'application/json' } });
        if (!response.ok) throw new Error('Hiba a készlet ellenőrzésekor');
        const termek = await response.json();

        const termekIndex = kosarTartalom.findIndex(t => t.tid === tid);
        const kosarbanMarVan = termekIndex !== -1 ? kosarTartalom[termekIndex].mennyiseg : 0;

        if (termek.Mennyiseg < kosarbanMarVan + 1) {
            ertesitesMegjelenites(`Nincs elég ${nev}. Elérhető: ${termek.Mennyiseg} db`, 'hiba');
            return;
        }

        if (termekIndex !== -1) {
            kosarTartalom[termekIndex].mennyiseg += 1;
        } else {
            kosarTartalom.push({ nev, ar, tid, mennyiseg: 1 });
        }

        elem('kosar-szam').textContent = kosarTartalom.reduce((total, item) => total + item.mennyiseg, 0);
        kosarKiemeles();
    } catch (hiba) {
        console.error('Készlet ellenőrzési hiba:', hiba);
        ertesitesMegjelenites('Hiba a készlet ellenőrzése során.', 'hiba');
    }
};

// Mennyiség növelése
const mennyisegNovel = async (tid) => {
    const termekIndex = kosarTartalom.findIndex(t => t.tid === tid);
    if (termekIndex === -1) return;

    try {
        const response = await fetch(`${API_URL}/Termek/${tid}`, { headers: { 'Accept': 'application/json' } });
        if (!response.ok) throw new Error('Hiba a készlet ellenőrzésekor');
        const termek = await response.json();

        if (termek.Mennyiseg < kosarTartalom[termekIndex].mennyiseg + 1) {
            ertesitesMegjelenites(`Nincs elég ${kosarTartalom[termekIndex].nev}. Elérhető: ${termek.Mennyiseg} db`, 'hiba');
            return;
        }

        kosarTartalom[termekIndex].mennyiseg += 1;
        elem('kosar-szam').textContent = kosarTartalom.reduce((total, item) => total + item.mennyiseg, 0);
        kosarMegnyitas();
    } catch (hiba) {
        console.error('Készlet ellenőrzési hiba:', hiba);
        ertesitesMegjelenites('Hiba a készlet ellenőrzése során.', 'hiba');
    }
};

// Mennyiség csökkentése
const mennyisegCsokkent = (tid) => {
    const termekIndex = kosarTartalom.findIndex(t => t.tid === tid);
    if (termekIndex === -1) return;

    kosarTartalom[termekIndex].mennyiseg -= 1;
    if (kosarTartalom[termekIndex].mennyiseg <= 0) {
        kosarTartalom.splice(termekIndex, 1);
    }

    elem('kosar-szam').textContent = kosarTartalom.reduce((total, item) => total + item.mennyiseg, 0);
    kosarMegnyitas();
};

// Kosár gomb kiemelése
const kosarKiemeles = () => {
    const kosarGomb = document.querySelector('.menu .kosar');
    if (kosarGomb) {
        kosarGomb.classList.add('ragyogas');
        setTimeout(() => kosarGomb.classList.remove('ragyogas'), 500);
    }
};

// Kosár ablak megnyitása
const kosarMegnyitas = () => {
    const fatyol = elem('fatyol');
    if (!fatyol) return;

    let fizetesiAblak = document.querySelector('.fizetesi-ablak');
    if (!fizetesiAblak) {
        fizetesiAblak = document.createElement('div');
        fizetesiAblak.className = 'fizetesi-ablak';
        fizetesiAblak.innerHTML = `
            <span class="bezar" onclick="ablakBezar()">×</span>
            <h2>Kosár</h2>
            <div id="kosar-tetelek"></div>
            <div class="osszesen">Összesen: <span id="osszesen-osszeg">0</span> Ft</div>
            <textarea id="fizetesi-megjegyzes" placeholder="Megjegyzés hozzáadása..." rows="3"></textarea>
            <button class="fizetesi-gomb" onclick="rendelesLeadas()">Rendelés leadása</button>
        `;
        fatyol.appendChild(fizetesiAblak);
    }

    const kosarTetelek = elem('kosar-tetelek');
    const osszesenOsszeg = elem('osszesen-osszeg');
    let osszesen = 0;

    kosarTetelek.innerHTML = kosarTartalom.length === 0 ? '<p>A kosár üres.</p>' : kosarTartalom.map(tetel => {
        const tetelOsszeg = tetel.ar * tetel.mennyiseg;
        osszesen += tetelOsszeg;
        return `
            <div class="fizetesi-tetel">
                <span>${tetel.nev}</span>
                <div class="mennyiseg-vezerlok">
                    <button class="mennyiseg-gomb" onclick="mennyisegCsokkent(${tetel.tid})">-</button>
                    <span class="mennyiseg">${tetel.mennyiseg}</span>
                    <button class="mennyiseg-gomb" onclick="mennyisegNovel(${tetel.tid})">+</button>
                </div>
                <span>${tetelOsszeg} Ft</span>
                <span class="torles" onclick="kosarbolTorles(${tetel.tid})">🗑️</span>
            </div>
        `;
    }).join('');

    osszesenOsszeg.textContent = osszesen.toFixed(2);
    fatyol.style.display = 'flex';
    fizetesiAblak.classList.add('nyitva');
};

// Tétel törlése a kosárból
const kosarbolTorles = tid => {
    kosarTartalom = kosarTartalom.filter(tetel => tetel.tid !== tid);
    elem('kosar-szam').textContent = kosarTartalom.reduce((total, item) => total + item.mennyiseg, 0);
    kosarMegnyitas();
};

// Kosár ablak bezárása
const ablakBezar = () => {
    const fatyol = elem('fatyol');
    if (fatyol) {
        fatyol.style.display = 'none';
        document.querySelector('.fizetesi-ablak')?.classList.remove('nyitva');
    }
};

// Rendelés leadása
const rendelesLeadas = async () => {
    const felhasznaloEmail = localStorage.getItem('felhasznaloEmail');
    if (!felhasznaloEmail) {
        ertesitesMegjelenites('Kérjük, jelentkezzen be a rendeléshez!', 'hiba');
        profilMegnyitas(true);
        return;
    }

    if (!kosarTartalom.length) {
        ertesitesMegjelenites('A kosár üres!', 'hiba');
        return;
    }

    const termekMennyisegek = kosarTartalom.reduce((acc, tetel) => {
        acc[tetel.tid] = { nev: tetel.nev, mennyiseg: tetel.mennyiseg };
        return acc;
    }, {});

    try {
        for (const tid in termekMennyisegek) {
            const response = await fetch(`${API_URL}/Termek/${tid}`, { headers: { 'Accept': 'application/json' } });
            if (!response.ok) throw new Error('Hiba a készlet ellenőrzésekor');
            const termek = await response.json();
            if (termek.Mennyiseg < termekMennyisegek[tid].mennyiseg) {
                ertesitesMegjelenites(`Nincs elég ${termekMennyisegek[tid].nev}. Elérhető: ${termek.Mennyiseg} db`, 'hiba');
                return;
            }
        }

        const kosarResponse = await fetch(`${API_URL}/Kosar`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                Email: felhasznaloEmail,
                Felnev: 'dolgozo1',
                Idopont: new Date().toISOString(),
                Megjegyzes: elem('fizetesi-megjegyzes')?.value || 'Nincs megjegyzés',
                Elkeszult: false
            })
        });

        if (!kosarResponse.ok) throw new Error('Hiba a kosár létrehozásakor');
        const { Kkod } = await kosarResponse.json();

        for (const tid in termekMennyisegek) {
            const checkResponse = await fetch(`${API_URL}/Kosarba/${Kkod}/${tid}`, { headers: { 'Accept': 'application/json' } });
            const method = checkResponse.ok ? 'PUT' : 'POST';
            const url = `${API_URL}/Kosarba${method === 'PUT' ? `/${Kkod}/${tid}` : ''}`;

            const response = await fetch(url, {
                method: method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Kkod, Tid: parseInt(tid), Tmenny: termekMennyisegek[tid].mennyiseg })
            });

            if (!response.ok) {
                const errorMessage = await response.text();
                throw new Error(`Hiba a kosár tétel ${method === 'POST' ? 'hozzáadásakor' : 'frissítésekor'}: ${errorMessage}`);
            }
        }

        ertesitesMegjelenites('Sikeres rendelés!', 'sikeres');
        kosarTartalom = [];
        elem('kosar-szam').textContent = '0';
        const fizetesiMegjegyzes = elem('fizetesi-megjegyzes');
        if (fizetesiMegjegyzes) fizetesiMegjegyzes.value = '';
        ablakBezar();
        await etelekMegjelenitese();
        szuresFrissites();
    } catch (hiba) {
        console.error('Rendelési hiba:', hiba);
        ertesitesMegjelenites(`Hiba a rendelés leadása során: ${hiba.message}`, 'hiba');
    }
};

// Profil ablak megnyitása
const profilMegnyitas = async (bejelentkezes = false) => {
    const fatyol = elem('profilFatyol');
    const ablak = document.querySelector('.profil-ablak');
    const tartalom = elem('profilTartalom');
    const bejelentkezve = localStorage.getItem('bejelentkezve') === 'true';

    if (!fatyol || !ablak || !tartalom) return;

    fatyol.style.display = 'flex';
    ablak.classList.add('nyitva');

    if (bejelentkezes || !bejelentkezve) {
        tartalom.innerHTML = `
            <h2>Bejelentkezés</h2>
            <form id="bejelentkezoUrlap">
                <label for="email">Email:</label>
                <input type="email" id="email" required>
                <label for="jelszo">Jelszó:</label>
                <input type="password" id="jelszo" required>
                <button type="submit">Bejelentkezés</button>
                <button type="button" onclick="regisztracioMegnyitas()">Regisztráció</button>
            </form>
        `;

        document.getElementById('bejelentkezoUrlap').addEventListener('submit', async e => {
            e.preventDefault();
            const email = document.getElementById('email').value;
            const jelszo = document.getElementById('jelszo').value;

            try {
                const controller = new AbortController();
                setTimeout(() => controller.abort(), 5000);
                const response = await fetch(`${API_URL}/Vasarlo/Login`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email, jelszo }),
                    signal: controller.signal
                });

                const eredmeny = await response.json();
                if (response.ok && eredmeny.success) {
                    localStorage.setItem('bejelentkezve', 'true');
                    localStorage.setItem('felhasznaloEmail', email);
                    localStorage.setItem('felhasznaloNev', eredmeny.nev || 'Felhasználó');
                    ertesitesMegjelenites('Sikeres bejelentkezés!', 'sikeres');
                    interakciokEngedelyezese();
                    profilBezar();
                    profilMegnyitas();
                } else {
                    ertesitesMegjelenites(eredmeny.message || 'Hibás email vagy jelszó!', 'hiba');
                }
            } catch (hiba) {
                console.error('Bejelentkezési hiba:', hiba);
                ertesitesMegjelenites('Hiba a bejelentkezés során.', 'hiba');
            }
        });
    } else {
        const felhasznaloEmail = localStorage.getItem('felhasznaloEmail') || 'Nincs email';
        const felhasznaloNev = localStorage.getItem('felhasznaloNev') || 'Felhasználó';

        let aktivRendelesekHtml = '<h3>Aktív rendelések</h3><div class="rendeles-lista">';
        try {
            const response = await fetch(`${API_URL}/Kosar`, { headers: { 'Accept': 'application/json' } });
            if (!response.ok) throw new Error('Hiba az aktív rendelések lekérésekor');
            const kosarak = await response.json();
            const userKosarak = kosarak.filter(k => k.Email === felhasznaloEmail);

            if (!userKosarak.length) {
                aktivRendelesekHtml += '<p>Nincsenek aktív rendelések.</p>';
            } else {
                userKosarak.forEach((kosar, index) => {
                    const datum = new Date(kosar.Idopont).toLocaleString('hu-HU');
                    const allapotSzoveg = kosar.Elkeszult ? 'Elkészült' : 'Feldolgozás alatt';
                    const allapotSzin = kosar.Elkeszult ? 'green' : 'red';
                    aktivRendelesekHtml += `
                        <div class="rendeles-tetel">
                            <div class="rendeles-fejlec" onclick="rendelesReszletekValtas(${index}, 'aktiv')">
                                <span>Rendelés - ${datum}</span>
                                <span class="valtas-ikon">▼</span>
                            </div>
                            <div class="rendeles-reszletek" id="aktiv-rendeles-reszletek-${index}">
                                <p><strong>Megjegyzés:</strong> ${kosar.Megjegyzes || 'Nincs'}</p>
                                <p><strong>Állapot:</strong> <span style="color: ${allapotSzin}">${allapotSzoveg}</span></p>
                            </div>
                        </div>
                    `;
                });
            }
        } catch (hiba) {
            console.error('Aktív rendelések betöltési hiba:', hiba);
            aktivRendelesekHtml += '<p style="color:red;">Hiba az aktív rendelések betöltésekor.</p>';
        }
        aktivRendelesekHtml += '</div>';

        let korabbiRendelesekHtml = '<h3>Korábbi rendelések</h3><div class="rendeles-lista">';
        try {
            const response = await fetch(`${API_URL}/Szamla`, { headers: { 'Accept': 'application/json' } });
            if (!response.ok) throw new Error('Hiba a korábbi rendelések lekérésekor');
            const szamlak = await response.json();
            const userSzamlak = szamlak.filter(sz => sz.Email === felhasznaloEmail);

            if (!userSzamlak.length) {
                korabbiRendelesekHtml += '<p>Nincsenek korábbi rendelések.</p>';
            } else {
                userSzamlak.forEach((szamla, index) => {
                    korabbiRendelesekHtml += `
                        <div class="rendeles-tetel">
                            <div class="rendeles-fejlec" onclick="rendelesReszletekValtas(${index}, 'elozo')">
                                <span>Rendelés #${szamla.Szkod}</span>
                                <span class="valtas-ikon">▼</span>
                            </div>
                            <div class="rendeles-reszletek" id="elozo-rendeles-reszletek-${index}">
                                <p><strong>Összeg:</strong> ${szamla.Osszeg} Ft</p>
                                <p><strong>Email:</strong> ${szamla.Email}</p>
                            </div>
                        </div>
                    `;
                });
            }
        } catch (hiba) {
            console.error('Korábbi rendelések betöltési hiba:', hiba);
            korabbiRendelesekHtml += '<p style="color:red;">Hiba a rendelések betöltésekor.</p>';
        }
        korabbiRendelesekHtml += '</div>';

        tartalom.innerHTML = `
            <h2>Profil</h2>
            <form id="profilFrissitoUrlap">
                <label for="profilNev">Név:</label>
                <input type="text" id="profilNev" value="${felhasznaloNev}">
                <label for="profilEmail">Email:</label>
                <input type="email" id="profilEmail" value="${felhasznaloEmail}" disabled>
                <label for="ujJelszo">Új jelszó:</label>
                <input type="password" id="ujJelszo">
                <button type="submit">Mentés</button>
            </form>
            ${aktivRendelesekHtml}
            ${korabbiRendelesekHtml}
            <button onclick="kijelentkezes()">Kijelentkezés</button>
        `;

        document.getElementById('profilFrissitoUrlap').addEventListener('submit', async e => {
            e.preventDefault();
            await profilFrissites();
        });
    }
};

// Rendelés részletek váltása
const rendelesReszletekValtas = (index, tipus) => {
    const reszletek = document.getElementById(`${tipus}-rendeles-reszletek-${index}`);
    const valtasIkon = document.querySelectorAll(`.rendeles-lista .rendeles-fejlec .valtas-ikon`)[index];
    if (reszletek && valtasIkon) {
        reszletek.style.display = reszletek.style.display === 'block' ? 'none' : 'block';
        valtasIkon.textContent = reszletek.style.display === 'block' ? '▲' : '▼';
    }
};

// Regisztrációs ablak megnyitása
const regisztracioMegnyitas = () => {
    const tartalom = elem('profilTartalom');
    if (!tartalom) return;

    tartalom.innerHTML = `
        <h2>Regisztráció</h2>
        <form id="regisztraciosUrlap">
            <label for="regNev">Név:</label>
            <input type="text" id="regNev" required>
            <label for="regEmail">Email:</label>
            <input type="email" id="regEmail" required>
            <label for="regJelszo">Jelszó:</label>
            <input type="password" id="regJelszo" required>
            <label for="regJelszoUjra">Jelszó újra:</label>
            <input type="password" id="regJelszoUjra" required>
            <button type="submit">Regisztráció</button>
            <button type="button" onclick="profilMegnyitas(true)">Vissza a bejelentkezéshez</button>
        </form>
    `;

    document.getElementById('regisztraciosUrlap').addEventListener('submit', async e => {
        e.preventDefault();
        const nev = document.getElementById('regNev').value;
        const email = document.getElementById('regEmail').value;
        const jelszo = document.getElementById('regJelszo').value;
        const jelszoUjra = document.getElementById('regJelszoUjra').value;

        if (jelszo !== jelszoUjra) {
            ertesitesMegjelenites('A jelszavak nem egyeznek!', 'hiba');
            return;
        }
        if (jelszo.length < 8) {
            ertesitesMegjelenites('A jelszónak legalább 8 karakteresnek kell lennie!', 'hiba');
            return;
        }
        if (!/[A-Z]/.test(jelszo)) {
            ertesitesMegjelenites('A jelszónak tartalmaznia kell nagybetűt!', 'hiba');
            return;
        }
        if (!/[a-z]/.test(jelszo)) {
            ertesitesMegjelenites('A jelszónak tartalmaznia kell kisbetűt!', 'hiba');
            return;
        }
        if (!/[!@#$%^&*(),.?":;{}]/.test(jelszo)) {
            ertesitesMegjelenites('A jelszónak tartalmaznia kell speciális karaktert!', 'hiba');
            return;
        }

        try {
            const response = await fetch(`${API_URL}/Vasarlo/Register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ nev, email, jelszo })
            });
            const eredmeny = await response.json();

            if (response.ok && eredmeny.success) {
                localStorage.setItem('bejelentkezve', 'true');
                localStorage.setItem('felhasznaloEmail', email);
                localStorage.setItem('felhasznaloNev', nev);
                ertesitesMegjelenites('Sikeres regisztráció!', 'sikeres');
                interakciokEngedelyezese();
                profilBezar();
                profilMegnyitas();
            } else {
                ertesitesMegjelenites(eredmeny.message || 'Hiba a regisztráció során!', 'hiba');
            }
        } catch (hiba) {
            console.error('Regisztrációs hiba:', hiba);
            ertesitesMegjelenites('Hiba a regisztráció során!', 'hiba');
        }
    });
};

// Profil ablak bezárása
const profilBezar = () => {
    const fatyol = elem('profilFatyol');
    if (fatyol) {
        fatyol.style.display = 'none';
        document.querySelector('.profil-ablak')?.classList.remove('nyitva');
    }
};

// Profil frissítése
const profilFrissites = async () => {
    const nev = document.getElementById('profilNev').value;
    const email = document.getElementById('profilEmail').value;
    const ujJelszo = document.getElementById('ujJelszo').value;

    if (!nev.trim()) {
        ertesitesMegjelenites('A név mező nem lehet üres!', 'hiba');
        return;
    }

    const frissitesiAdatok = { Email: email, Nev: nev };
    if (ujJelszo.trim()) {
        if (ujJelszo.length < 8) {
            ertesitesMegjelenites('A jelszónak legalább 8 karakteresnek kell lennie!', 'hiba');
            return;
        }
        if (!/[A-Z]/.test(ujJelszo)) {
            ertesitesMegjelenites('A jelszónak tartalmaznia kell nagybetűt!', 'hiba');
            return;
        }
        if (!/[a-z]/.test(ujJelszo)) {
            ertesitesMegjelenites('A jelszónak tartalmaznia kell kisbetűt!', 'hiba');
            return;
        }
        if (!/[!@#$%^&*(),.?":;{}]/.test(ujJelszo)) {
            ertesitesMegjelenites('A jelszónak tartalmaznia kell speciális karaktert!', 'hiba');
            return;
        }
        frissitesiAdatok.Jelszo = ujJelszo;
    }

    try {
        const response = await fetch(`${API_URL}/Vasarlo/Update`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(frissitesiAdatok)
        });

        if (!response.ok) throw new Error('Hiba a profil frissítésekor');
        localStorage.setItem('felhasznaloNev', nev);
        ertesitesMegjelenites('Profil sikeresen frissítve!', 'sikeres');
        profilBezar();
        profilMegnyitas();
    } catch (hiba) {
        console.error('Profil frissítési hiba:', hiba);
        ertesitesMegjelenites('Hiba a profil frissítése során.', 'hiba');
    }
};

// Kijelentkezés
const kijelentkezes = () => {
    localStorage.setItem('bejelentkezve', 'false');
    localStorage.removeItem('felhasznaloEmail');
    localStorage.removeItem('felhasznaloNev');
    ertesitesMegjelenites('Sikeres kijelentkezés!', 'sikeres');
    interakciokTiltasa();
    profilBezar();
    profilMegnyitas(true);
};

// Téma váltása
const temaValtas = () => {
    document.body.classList.toggle('vilagos-tema');
    const temaIkon = document.querySelector('#tema-valtas i');
    const oldalsavTemaIkon = document.querySelector('#oldalsav-tema i');
    if (temaIkon) {
        temaIkon.className = document.body.classList.contains('vilagos-tema') ? 'fas fa-sun' : 'fas fa-moon';
    }
    if (oldalsavTemaIkon) {
        oldalsavTemaIkon.className = document.body.classList.contains('vilagos-tema') ? 'fas fa-sun' : 'fas fa-moon';
    }
};

// Képgaléria inicializálása
const galeriaInditasa = () => {
    const galeria = document.querySelector('.galeria');
    const diak = document.querySelector('.galeria-dia');
    const szovegek = document.querySelectorAll('.galeria-szoveg p');
    const kepek = document.querySelectorAll('.galeria-dia img');

    if (!galeria || !diak || !szovegek || !kepek.length) return;

    const diaSzam = kepek.length;
    let atmenetFolyamatban = false;
    let autoDiaIntervallum;

    const elsoKlon = kepek[0].cloneNode(true);
    const utolsoKlon = kepek[diaSzam - 1].cloneNode(true);
    diak.appendChild(elsoKlon);
    diak.insertBefore(utolsoKlon, kepek[0]);

    diak.style.transform = `translateX(-${100 * 1}%)`;

    const galeriaFrissites = (index, animacio = true) => {
        if (atmenetFolyamatban) return;
        atmenetFolyamatban = true;

        diak.style.transition = animacio ? 'transform 0.5s ease-in-out' : 'none';
        diak.style.transform = `translateX(-${100 * (index + 1)}%)`;

        szovegek.forEach((szoveg, i) => szoveg.classList.toggle('aktiv', i === index % diaSzam));

        if (index === -1) {
            setTimeout(() => {
                diak.style.transition = 'none';
                galeriaIndex = diaSzam - 1;
                diak.style.transform = `translateX(-${100 * (galeriaIndex + 1)}%)`;
                atmenetFolyamatban = false;
            }, 500);
        } else if (index === diaSzam) {
            setTimeout(() => {
                diak.style.transition = 'none';
                galeriaIndex = 0;
                diak.style.transform = `translateX(-${100 * (galeriaIndex + 1)}%)`;
                atmenetFolyamatban = false;
            }, 500);
        } else {
            galeriaIndex = index;
            setTimeout(() => atmenetFolyamatban = false, 500);
        }
    };

    const autoDiaInditas = () => {
        autoDiaIntervallum = setInterval(() => {
            galeriaFrissites(galeriaIndex + 1);
        }, 5000);
    };

    const autoDiaLeallitas = () => {
        clearInterval(autoDiaIntervallum);
    };

    autoDiaInditas();

    galeria.addEventListener('mouseenter', autoDiaLeallitas);
    galeria.addEventListener('mouseleave', autoDiaInditas);

    galeriaFrissites(galeriaIndex, false);
};

// Aktuális idő frissítése
const idopontFrissites = () => {
    const idopontElem = elem('aktualis-idopont');
    if (idopontElem) {
        idopontElem.textContent = new Date().toLocaleString('hu-HU', {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
    }
};

// Hamburger menü inicializálása
const hamburgerMenuInicializalas = () => {
    const hamburger = elem('hamburger');
    const oldalsav = document.querySelector('.oldalsav');
    const tartalom = document.querySelector('.tartalom');

    if (!hamburger || !oldalsav || !tartalom) return;

    const hamburgerAllapotFrissites = () => {
        const mobilNezet = window.innerWidth <= 768;

        if (!mobilNezet) {
            hamburger.classList.remove('nyitott');
            oldalsav.classList.remove('aktiv');
            tartalom.classList.remove('eltolva');
        }
    };

    const oldalsavValtas = () => {
        if (window.innerWidth > 768) return;

        hamburger.classList.toggle('nyitott');
        oldalsav.classList.toggle('aktiv');
        tartalom.classList.toggle('eltolva');
    };

    hamburger.addEventListener('click', oldalsavValtas);
    window.addEventListener('resize', hamburgerAllapotFrissites);
    hamburgerAllapotFrissites();
};

// Eseménykezelők beállítása
const esemenyKezelokBeallitasa = () => {
    const fatyol = elem('fatyol');
    if (fatyol) fatyol.addEventListener('click', e => e.target === fatyol && ablakBezar());

    const profilFatyol = elem('profilFatyol');
    if (profilFatyol) profilFatyol.addEventListener('click', e => e.target === profilFatyol && profilBezar());

    const keresoMezo = elem('kereso-mezo');
    if (keresoMezo) keresoMezo.addEventListener('input', szuresFrissites);

    const profilGomb = elem('profilGomb');
    if (profilGomb) profilGomb.addEventListener('click', () => profilMegnyitas());

    const temaValto = elem('tema-valtas');
    if (temaValto) temaValto.addEventListener('click', temaValtas);

    const oldalsavTema = elem('oldalsav-tema');
    if (oldalsavTema) oldalsavTema.addEventListener('click', temaValtas);

    const oldalsavProfil = elem('oldalsav-profil');
    if (oldalsavProfil) oldalsavProfil.addEventListener('click', () => {
        profilMegnyitas();
        document.querySelector('.oldalsav').classList.remove('aktiv');
        document.querySelector('.hamburger').classList.remove('nyitott');
        document.querySelector('.tartalom').classList.remove('eltolva');
    });

    const oldalsavKijelentkezes = elem('oldalsav-kijelentkezes');
    if (oldalsavKijelentkezes) oldalsavKijelentkezes.addEventListener('click', () => {
        kijelentkezes();
        document.querySelector('.oldalsav').classList.remove('aktiv');
        document.querySelector('.hamburger').classList.remove('nyitott');
        document.querySelector('.tartalom').classList.remove('eltolva');
    });
};