import axios from 'axios'
import jwt from 'jsonwebtoken'

export const requestSaveConfiguration = (name, configurationId, selectedOptions) => {
    return saveConfigTest(name, selectedOptions)

    // return new Promise((resolve, reject) => {
    //     const data = {
    //         savedName: name,
    //         selectedOptions: selectedOptions
    //     }
    //     axios.post(`/account/configurations/${configurationId}`, data)
    //     .then(res => {
    //         resolve(res.data)
    //     })
    //     .catch(err => {
    //         reject('Could not save configuration!')
    //     })
    // })
}
export const fetchAllSavedConfigurations = () => {
    return fetchConfigsTest()

    // return new Promise((resolve, reject) => {
    //     axios.get(`/account/configurations/`)
    //     .then(res => {
    //         resolve(res.data)
    //     })
    //     .catch(err => {
    //         reject('Could not get saved configurations!')
    //     })
    // })
}

export const requestRegister = (username, email, password) => {
    return new Promise((resolve, reject) => {
        reject('Not implemented')
    })
}

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


// A mock function to mimic making an async request for the login
function loginTest(username, password) {
    return new Promise((resolve, reject) => {
        // reject('INVALID CREDENTIALS)
        setTimeout(() => {
            // user token
            let token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6InRlc3R1c2VyIiwiZW1haWwiOiJ0ZXN0dXNlckB0ZXN0LWZ1Y2hzLmNvbSIsImFkbWluIjpmYWxzZSwiaWF0IjoxNjE3NDQ5MDIyfQ.qi6WUK7Pct6WjBZfm-J5f8cDmE4M1oagEJaxOHntFSs'
            
            // admin token
            if (username === 'admin') {
                token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6ImFkbWluIiwiZW1haWwiOiJjb25maWd1cmF0b3ItYWRtaW5AdGVzdC1mdWNocy5jb20iLCJhZG1pbiI6dHJ1ZSwiaWF0IjoxNjE3NDQ5MDIyfQ.vSJDmhZ5-2lCVDRSXbIFTK3RkFxPDMYZOhYZOzN5gnQ'
            }
            
            const user = jwt.decode(token)

            resolve({token, user})
        }, 100)
    })
}

function saveConfigTest(name, options) {
    return new Promise((resolve, reject) => {
        resolve('Successfully saved ' + name)
    })
}
function fetchConfigsTest() {
    return new Promise((resolve, reject) => {
        const data = [
            {
                savedName: "TestConfig",
                status: "saved", // or "ordered"
                name: "Car",
                description: "This is a car",
                selectedOptions: ['YELLOW']
            }
        ]
        resolve(data)
    })
}