import axios from 'axios'
import jwt from 'jsonwebtoken'

export const requestLogin = (username, password) => {
    return loginTest(username, password)

    // return new Promise((resolve, reject) => {
    //     axios.post('/api/auth', {username, password})
    //     .then(res => {
    //         const token = res.data.token
    //         const user = jwt.decode(token)
    //         resolve({token, user})
    //     })
    //     .catch(err => {
    //         reject('Invalid Credentials')
    //     })
    // })
}

export const setAuthorizationToken = (token) => {
    if (token) {
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
    } else {
        delete axios.defaults.headers.common['Authorization']
    }
}


function loginTest(username, password) {
    return new Promise((resolve, reject) => {
        // reject('INVALID CREDENTIALS)
        setTimeout(() => {
            // user token
            // const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6InRlc3R1c2VyIiwiZW1haWwiOiJ0ZXN0dXNlckB0ZXN0LWZ1Y2hzLmNvbSIsImFkbWluIjpmYWxzZSwiaWF0IjoxNjE3NDQ5MDIyfQ.qi6WUK7Pct6WjBZfm-J5f8cDmE4M1oagEJaxOHntFSs'
            
            // admin token
            const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImFkbWluIiwiZW1haWwiOiJjb25maWd1cmF0b3ItYWRtaW5AdGVzdC1mdWNocy5jb20iLCJhZG1pbiI6dHJ1ZSwiaWF0IjoxNjE3NDQ5MDIyfQ.vSJDmhZ5-2lCVDRSXbIFTK3RkFxPDMYZOhYZOzN5gnQ'
            
            const user = jwt.decode(token)

            resolve({token, user})
        }, 100)
    })
}