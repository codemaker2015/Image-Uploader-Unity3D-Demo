const express = require('express')
const fileUpload = require('express-fileupload')
const path = require('path')
const app = express()
app.use(fileUpload())
app.use(express.static('public'))
app.use(express.urlencoded({ extended: true }))

app.get('/', (req, res) => res.send('Upload server 1.0'))

app.post('/uploads', function(req, res) {
    const file = req.files.image
    const filePath = path.join(__dirname, 'public', 'images', `${file.name}`)
  
    file.mv(filePath, err => {
        if (err) return res.status(500).send(err)
        res.send("File upload successfully")
    })
  })

app.listen(3000, (req, res)=> {
  console.log("Server is running on port 3000")
})