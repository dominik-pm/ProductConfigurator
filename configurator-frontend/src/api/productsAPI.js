import axios from 'axios'
import { baseURL, LOCAL_DATA } from './general'

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
            reject('Ordering not available in test mode!')
            // resolve()
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
            reject('API not reachable')
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
        images: ['vw-golf-r-2021.jpg']
    },
    // {
    //     configId: 1,
    //     name: 'Computer',
    //     description: 'high end computer',
    //     images: ['Computer0.jpg']
    // },
    // {
    //     configId: 2,
    //     name: 'watch',
    //     description: 'entry level watch',
    //     images: ['Watch0.jpg']
    // },
    // {
    //     configId: 3,
    //     name: 'watch',
    //     description: 'entry level watch',
    //     images: ['Watch0.jpg']
    // },
    // {
    //     configId: 4,
    //     name: 'watch',
    //     description: 'entry level watch',
    //     images: ['Watch0.jpg']
    // },
    // {
    //     configId: 5,
    //     name: 'Computer',
    //     description: 'high end computer',
    //     images: ['Computer0123.jpg']
    // }
]