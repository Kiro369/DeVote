console.log('api pagination test');

let itemsDiv = document.querySelector('#items');
let paginationDiv = document.querySelector('#pagination');
let paginationFeedBackDiv = document.querySelector('#paginationFeedBack');
let paginationRequestMetadatDiv = document.querySelector('#paginationRequestMetadata');

let scrolled = false;
let intervalIDList = [];

// console.log(paginationFeedBackDiv)
// console.log("paginationDiv", paginationDiv);
// console.log("paginationDiv", itemsDiv);
// console.log("paginationRequestMetadatDiv", paginationRequestMetadatDiv);

async function testAPIPagination(baseAPI) {
    let limitInput = document.querySelector("#limitInput").value || 20
    let radios = Array.from(document.querySelectorAll(".itemType"));
    let resourceType;
    radios.forEach((radio, index) => {
        // console.log(index)
        if (radio.checked) {
            console.log(radio.value)
            resourceType = radio.value;
        }
    })

    if (resourceType == "block") await updateItemsUI(baseAPI, resourceType, limitInput, "initial")
    if (resourceType == "transaction") await updateItemsUI(baseAPI, resourceType, limitInput, "initial")
}
async function fetchItems(api) {
    console.log("fetchItems api", api)
    let response = await fetch(api)
    return await response.json()
}

function scrollDown() {
    scrolled = false;
    paginationRequestMetadatDiv.focus();
    paginationRequestMetadatDiv.scrollTop = paginationRequestMetadatDiv.scrollHeight;

}

// intervalID = setInterval(scrollDown, 800)

async function updateItemsUI(baseAPI, resourceType, limit, requestType) {

    let intervalID = setInterval(scrollDown, 100)
    intervalIDList.push(intervalID)

    let api = baseAPI;
    if (requestType == "initial") api = `${baseAPI}/${resourceType}s?limit=${limit}`;

    const p = document.createElement('p')
    p.innerText = `Requesting : ${api}`
    paginationRequestMetadatDiv.append(p);

    let resultJSON = await fetchItems(api);

    if (!resultJSON) return;

    const { result } = resultJSON;
    if (result) {
        p.innerText += `
        Response : ${result.length} ${resourceType}s`
        paginationRequestMetadatDiv.append(p);
    }


    const pList = [];

    result.forEach(element => {
        if (resourceType == "transaction") {
            const { BlockHeight, Date, Elected, Elector, Hash } = element
            const p = document.createElement('p')
            p.innerText = `
            Date : ${Date} 
            Elected : ${Elected}
            Elector : ${Elector}
            `
            // BlockHeight : ${BlockHeight}
            pList.push(p)
        }
        if (resourceType == "block") {
            const { Height, Hash, Miner } = element
            const p = document.createElement('p')
            p.innerText = `
            Height : ${Height} 
            Hash : ${Hash}
            `
            // Miner : ${Miner}
            pList.push(p)
        }

    });
    itemsDiv.innerHTML = pList.map(el => el.innerText).join("</br>");

    if (resultJSON) {
        const { pagination, result } = resultJSON;
        console.log(pagination)
        console.log(result)

        const { more, prev, next, max } = pagination;

        paginationDiv.innerHTML = "";
        let cursorParamName = resourceType == "block" ? "height" : "timestamp"
        let maxValue = resourceType == "block" ? max : `${max} - ${new Date(max).toLocaleString()}`;
        paginationFeedBackDiv.innerHTML = `
        latest ${resourceType} ${cursorParamName} : ${maxValue}
        <br>
        more : ${more}
        `;

        if (next) {
            const nextBTN = createBTN(next, "next", resourceType);
            paginationDiv.append(nextBTN);
            p.innerText += `
            Next URL : ${next}`
            paginationRequestMetadatDiv.append(p);
        }

        if (prev) {
            const prevBTN = createBTN(prev, "prev", resourceType);
            paginationDiv.append(prevBTN);
            p.innerText += `
            Prev URL : ${prev}`
            paginationRequestMetadatDiv.append(p);
        }

        if (!more) {
            paginationFeedBackDiv.innerHTML += `
            <br>
            <span style ="color:red">No more ${resourceType}s </span>`
        }


    }
}

function createBTN(paginationAPI, paginationDir, resourceType) {
    console.log("==============================================================================")
    // console.log("baseAPI", baseAPI)
    // console.log("cursorValue", cursorValue)
    // console.log("paginationDir", paginationDir)

    let btn = document.createElement('button')
    btn.innerText = `${paginationDir} ${resourceType}s`
    console.log("paginationAPI", paginationAPI)
    btn.addEventListener("click", async () => await updateItemsUI(paginationAPI, resourceType))
    console.log("==============================================================================")

    return btn
};

(async () => {
    console.log(window.location.origin)
    // let baseAPI = `http://localhost:3000`;

    let baseAPI = window.location.origin;

    let radios = Array.from(document.querySelectorAll(".itemType"));
    radios.forEach(radio => {
        radio.addEventListener("click", async () => {
            await testAPIPagination(baseAPI)
        })
    })

    paginationRequestMetadatDiv.addEventListener("scroll", () => {
        console.log("scrolled")
        intervalIDList.forEach(intervalID => clearInterval(intervalID))
        scrolled = true;
    })

})()
