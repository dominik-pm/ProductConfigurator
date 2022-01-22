import axios from 'axios'
import { baseURL } from './general'

export const fetchAll = () => {    
    return fetchApiTest()
}

export const postOrderConfiguredProduct = (configurationId, name, selectedOptions, price) => {
    return new Promise((resolve, reject) => {

        const data = {
            configurationName: name,
            options: selectedOptions,
            price
        }
        axios.post(`${baseURL}/products/${configurationId}`, data)
        .then(res => {
            resolve(res.data)
        })
        .catch(err => {
            reject(err)
        })

    })
}


// A mock function to mimic making an async request for data
function fetchApiTest(amount = products.length) {
    return new Promise((resolve, reject) =>
        // setTimeout(() => reject('AUTHENTICATION_FAILED'), 500)
        setTimeout(() => resolve({
            error: null,
            products
        }), 500)
    )
}

const products = [
    {
        id: "0",
        name: 'Car',
        description: 'a next generation automobile',
        image: 'Car0.jpg'
    },
    {
        id: 1,
        name: 'Computer',
        description: 'high end computer',
        image: 'Computer0.jpg'
    },
    {
        id: 2,
        name: 'watch',
        description: 'entry level watch',
        image: 'Watch0.jpg'
    },
    {
        id: 3,
        name: 'watch',
        description: 'entry level watch',
        image: 'Watch0.jpg'
    },
    {
        id: 4,
        name: 'watch',
        description: 'entry level watch',
        image: 'Watch0.jpg'
    },
    {
        id: 5,
        name: 'Computer',
        description: 'high end computer',
        image: 'Computer0123.jpg'
    }
]