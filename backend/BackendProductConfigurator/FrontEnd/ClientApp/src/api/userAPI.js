import axios from 'axios'
import { baseURL, LOCAL_DATA } from './general'
import jwt from 'jsonwebtoken'

export const requestSaveConfiguration = (configurationId, name, selectedOptions) => {
    if (LOCAL_DATA) {
        return saveConfigTest(name, selectedOptions)
    }

    return new Promise((resolve, reject) => {
        const data = {
            savedName: name,
            options: selectedOptions
        }
        axios.post(`${baseURL}/account/configurations/${configurationId}`, data)
        .then(res => {
            resolve(res.data)
        })
        .catch(err => {
            reject('Could not save configuration!')
        })
    })
}
export const requestDeleteSavedConfiguration = (configurationId, name) => {
    return new Promise((resolve, reject) => {
        // reject('Not implemented!')

        const data = {
            savedName: name
        }
        axios.delete(`${baseURL}/account/configurations/${configurationId}`, data)
        .then(res => {
            resolve(res.data)
        })
        .catch(err => {
            reject('Could not get saved configurations!')
        })
    })
}
export const fetchSavedConfigurations = () => {
    if (LOCAL_DATA) {
        return fetchConfigsTest()
    }

    return new Promise((resolve, reject) => {
        axios.get(`${baseURL}/account/configurations/`)
        .then(res => {
            resolve(res.data)
        })
        .catch(err => {
            reject('Could not get saved configurations!')
        })
    })
}
export const fetchAllOrderedConfigurations = () => {
    if (LOCAL_DATA) {
        return fetchOrderedConfigsTest()
    }

    return new Promise((resolve, reject) => {
        axios.get(`${baseURL}/account/allorderedconfigurations/`)
        .then(res => {
            resolve(res.data)
        })
        .catch(err => {
            reject('Could not get all ordered configurations!')
        })
    })
}

export const requestRegister = (username, email, password) => {
    return new Promise((resolve, reject) => {
        reject('Not implemented')

        // return new Promise((resolve, reject) => {
        //     axios.post(`https://test-fuchs.com/api/auth/register`, {username, email, password})
        //     .then(res => {
        //         // dont expect a token, if the client has to login after registering
        //         const token = res.data.token
        //         const user = jwt.decode(token)
        //         resolve({token, user})
        //     })
        //     .catch(err => {
        //         reject('User already exists!')
        //     })
        // })
    })
}

export const requestLogin = (username, password) => {
    return loginTest(username, password)

    // return new Promise((resolve, reject) => {
    //     axios.post(`https://test-fuchs.com/api/auth`, {username, password})
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


// A mock function to mimic making an async request for the login
function loginTest(username, password) {
    return new Promise((resolve, reject) => {
        if (username !== 'admin' && username !== 'user') {
            reject('INVALID CREDENTIALS')
        }
        
        setTimeout(() => {
            // user token
            let token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyTmFtZSI6InRlc3R1c2VyIiwidXNlckVtYWlsIjoidGVzdHVzZXJAdGVzdC1mdWNocy5jb20iLCJhZG1pbiI6ZmFsc2UsImlhdCI6MTYxNzQ0OTAyMn0.Q27mbjp1g6TMKQA33mbKNJawouIh5r7jIahpkQJDAyo'
            
            // admin token
            if (username === 'admin') {
                token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyTmFtZSI6ImFkbWluIiwidXNlckVtYWlsIjoiY29uZmlndXJhdG9yLWFkbWluQHRlc3QtZnVjaHMuY29tIiwiYWRtaW4iOnRydWUsImlhdCI6MTYxNzQ0OTAyMn0.q9t1l9YgFR8VeUF4PDFuooyuhQDDa7d8NmdtBNh-hVU'
            }
            
            const user = jwt.decode(token)

            resolve({token, user})
        }, 250)
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
                savedName: "Sport Extra",
                status: "saved", // or "ordered"
                date: new Date(1644407751516).toISOString(),
                configId: '0',
                name: "Auto",
                description: "Das Auto der n??chsten Generation",
                options: ['YELLOW']
            },
            // {
            //     savedName: "TestConfig2",
            //     status: "saved",
            //     date: new Date().toISOString(),
            //     configId: 0,
            //     name: "Car",
            //     description: "This is a car",
            //     options: ['BLUE', 'DIESEL', 'D150']
            // },
            {
                savedName: "Pers??nliche Konfiguration 1",
                status: "ordered",
                date: new Date(1644407751516).toISOString(),
                configId: '0',
                name: "Auto",
                description: "Das Auto der n??chsten Generation",
                options: ['YELLOW', 'DIESEL', 'D150', 'PANORAMAROOF', 'PANORAMASMALL']
            },
            // {
            //     savedName: "TestOrderedConfig2",
            //     status: "ordered",
            //     date: new Date().toISOString(),
            //     configId: 0,
            //     name: "Car",
            //     description: "This is a car",
            //     options: ['BLUE', 'PETROL', 'P220', 'HEATED_SEATS']
            // }
        ]
        resolve(data)
    })
}
function fetchOrderedConfigsTest() {
    return new Promise((resolve, reject) => {
        const data = [
            {
                savedName: "Pers??nliche Konfiguration 1",
                user: {
                    userName: "admin",
                    userEmail: "configurator-admin@test-fuchs.com"
                },
                date: new Date(1644407751516).toISOString(),
                configId: '0',
                name: "Auto",
                description: "Das Auto der n??chsten Generation",
                options: ['YELLOW', 'DIESEL', 'D150', 'PANORAMAROOF', 'PANORAMASMALL']
            }
            // {
            //     savedName: "TestOrderedConfig",
            //     userName: "admin",
            //     userEmail: "configurator-admin@test-fuchs.com",
            //     date: new Date().toISOString(),
            //     configId: '0',
            //     name: "Car",
            //     description: "This is a car",
            //     options: ['YELLOW', 'DIESEL', 'D150', 'PANORAMAROOF', 'PANORAMASMALL']
            // },
            // {
            //     savedName: "TestOrderedConfig2",
            //     userName: "admin",
            //     userEmail: "configurator-admin@test-fuchs.com",
            //     status: "ordered",
            //     date: new Date().toISOString(),
            //     configId: '0',
            //     name: "Car",
            //     description: "This is a car",
            //     options: ['BLUE', 'PETROL', 'P220', 'HEATED_SEATS']
            // }
        ]
        resolve(data)
    })
}