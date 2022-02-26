#!/usr/bin/env node

if (process.argv.length != 3) {
    console.log("Usage: block2hn <textpath>")
    console.log(process.argv.length)
    process.exit(1)
}

const path = require('path')
const fs = require('fs')
const {encode} = require('html-entities')

const filepath = process.argv[2]

const cont = fs.readFileSync( filepath )
const srcLines = cont.toString().split('\n')

class Paragraph {
    constructor(begin, end, text) {
        this.begin = begin
        this.end = end
        this.text = text
    }
}

const linesToParas = (lines) => {
    let begin = 0
    let paras = []
    let curlines = []
    for( let [idx, line] of lines.entries())
    {
        if (line == "")
        {
            paras.push( new Paragraph(begin, idx, curlines.join('<br>\n')))
            begin = idx+1
            curlines = []
        }
        else
        {
            curlines.push(encode(line))
        }
    }
    if (curlines.length != 0)
    {
        paras.push( new Paragraph(begin, lines.length, curlines.join('<br>\n')))
    }

    return paras
}

const paras = linesToParas(srcLines)

// <div class="hn-multi-sel box" hn-value="1,2">Hello</div>
const para2html = (para) => {
    return `<div class="hn-multi-sel box" hn-value="${para.begin},${para.end}">
              ${para.text}
            </div>`
}

const paras2html = (paras) => {
    return paras.map( p => para2html(p) ).join("\n")
}


const parahtml = paras2html(paras)
console.log(`<div class="buttons level-right">
<button class="button hn-cancel">Cancel</button>
<button class="button hn-submit">Archive</button>
</div>
`)
console.log(parahtml)
