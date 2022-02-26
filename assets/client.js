const sendMessage = (type, body) => {
    window.external.sendMessage(JSON.stringify({Type:type, Body: body}))
}

const dispatcher = new Map()

const onMsg = (type, callback) => {
    dispatcher[type] = callback
}

window.external.receiveMessage(message => {
    const msg = JSON.parse(message)
    dispatcher[msg.Type](msg.Body)
})

const setupMultiSelect = (root) => {
    Array.from(root.getElementsByClassName("hn-multi-sel")).map( elem=> {
        elem.addEventListener('click', ()=>{ elem.classList.toggle("is-active") })
    })
}

const onSubmit = (root)=> {
    const values = Array.from(root.getElementsByClassName("is-active")).map(elem => elem.getAttribute("hn-value"))
    sendMessage("notifyDone", JSON.stringify(values))
}

const setupSubmitCancel = (root) => {
    Array.from(root.getElementsByClassName("hn-submit")).map (elem => {
        elem.addEventListener('click', ()=>{
            onSubmit(root)
        })
    })
    Array.from(root.getElementsByClassName("hn-cancel")).map (elem => {
        elem.addEventListener('click', ()=>{ sendMessage("notifyCancel", "") })
    })
}

window.addEventListener('load', (e)=> {
    const contentRoot = document.getElementById('content-root')

    onMsg("showHtml", (html) => {
        contentRoot.innerHTML = html
        setupMultiSelect(contentRoot)
        setupSubmitCancel(contentRoot)
    })

    sendMessage("notifyLoaded", "")
})

