import axios from 'axios'
import { baseURL, LOCAL_DATA } from './general'

export const fetchAvailableImages = () => {
    return new Promise((resolve, reject) => {
        if (LOCAL_DATA) {
            resolve(['car.jpg', 'watch.png', 'computer.jpg'])
            return
        }
        
        axios.get(`${baseURL}/images`)
        .then(res => {
            if (!res.data) {
                console.log('no response data')
                resolve([])
            }
            console.log('loaded images: ', res.data)
            resolve(res.data)
        })
        .catch(err => {
            console.log(err.toString())
            reject(err.toString())
        })
    })
}

export const fetchAll = () => {
    if (LOCAL_DATA) {
        return fetchApiTest()
    }

    return new Promise((resolve, reject) => {
        axios.get(`${baseURL}/products`)
        .then(res => {
            if (!res.data) {
                console.log('no response data')
                resolve([])
            }
            console.log(res.data)
            resolve(res.data)
        })
        .catch(err => {
            console.log(err.toString())
            reject(err.toString())
        })
    })
}

export const postOrderConfiguredProduct = (configurationId, name, selectedOptions, price, model = '') => {
    return new Promise((resolve, reject) => {

        if (LOCAL_DATA) {
            // reject('Konfiguration ungültig!')
            resolve()
        }
        

        const data = {
            configurationName: name,
            options: selectedOptions,
            price: price,
            model
        }
        console.log(data)

        axios.post(`${baseURL}/products/${configurationId}`, data)
        .then(res => {
            resolve(res.data)
        })
        .catch(err => {
            console.log('err:', err)
            if (err.response) {
                reject(err.response.data.detail)
            } else if (err.request) {
                console.log(err.request)
            } else {
                reject('Api unreachable')
            }
        })

    })
}


// A mock function to mimic making an async request for data
function fetchApiTest(amount = products.length) {
    return new Promise((resolve, reject) =>
        // setTimeout(() => reject('AUTHENTICATION_FAILED'), 500)
        setTimeout(() => resolve(products), 500)
    )
}

const products = [
    {
        configId: '0',
        name: 'Auto',
        description: 'Das Auto der nächsten Generation',
        images: ['car.png']
    },
    {
        configId: 1,
        name: 'Computer',
        description: 'high end computer',
        images: ['computer.jpg']
    },
    {
        configId: 2,
        name: 'watch',
        description: 'entry level watch',
        images: ['watch.png']
    }
]