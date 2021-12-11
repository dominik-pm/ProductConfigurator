export const fetchId = (productId) => {
    return fetchApiTest(productId)
}

// A mock api request function to mimic making an async request for data
const testDelay = 0;
function fetchApiTest(configId) {
    return new Promise((resolve, reject) =>
        setTimeout(() => {

            const conf = configurations.find(c => c.id === configId)

            if (!conf) {
                reject('no configuration found')
            }
            if (!conf.options || !conf.optionGroups || !conf.optionSections || !conf.rules) {
                reject('Configuration invalid!')
            }

            resolve(conf)

        }, testDelay)
    )
}

const configurations = [
    {
        id: 0,
        name: 'Car',
        description: 'its a car, to drive from A to B',
        image: '1.jpg',
        options: [
            {
                id: 'BLUE',
                name: 'Blue',
                description: 'A blue color',
                image: ''
            },
            {
                id: 'YELLOW',
                name: 'Yellow',
                description: 'A yellow color',
                image: ''
            },
            {
                id: 'GREEN',
                name: 'Green',
                description: 'A green color',
                image: ''
            },
            {
                id: 'DIESEL',
                name: 'Diesel Motor',
                description: 'a motor driven by diesel fuel',
                image: ''
            },
            {
                id: 'PETROL',
                name: 'Petrol Motor',
                description: 'a motor driven by petrol fuel',
                image: ''
            },
            {
                id: 'D150',
                name: '150 PS Diesel Motor',
                description: 'a diesel V4 motor with 150 PS',
                image: ''
            },
            {
                id: 'D250',
                name: '250 PS Diesel Motor',
                description: 'a diesel V6 motor with 250 PS',
                image: ''
            },
            {
                id: 'P220',
                name: '220 PS Petrol Motor',
                description: 'a petrol V6 motor with 220 PS',
                image: ''
            },
            {
                id: 'P450',
                name: '450 PS Petrol Motor',
                description: 'a petrol V8 motor with 450 PS',
                image: ''
            },
            {
                id: 'PANORAMAROOF',
                name: 'Panoramic Roof',
                description: 'a glass roof for an open feeling',
                image: ''
            },
            {
                id: 'PANORAMASMALL',
                name: 'small',
                description: 'a small glass roof',
                image: ''
            },
            {
                id: 'PANORAMALARGE',
                name: 'large',
                description: 'a large glass roof for an amazing open feeling',
                image: ''
            }

        ],
        optionSections: [
            {
                id: 'EXTERIOR',
                name: 'Exterior',
                optionGroupIds: [
                    'COLOR_GROUP'
                ]
            },
            {
                id: 'MOTOR_SECTION',
                name: 'Motor',
                optionGroupIds: [
                    'MOTORTYPE_GROUP', 'MOTOR_GROUP'
                ]
            },
            {
                id: 'PANORAMA_SECTION',
                name: 'Panorama',
                optionGroupIds: [
                    'PANORAMA_GROUP', 'PANORAMATYPE_GROUP'
                ]
            }
        ],
        optionGroups: [
            {
                id: 'COLOR_GROUP',
                name: 'Color',
                description: 'the exterior color of the car',
                optionIds: [
                    'BLUE', 'YELLOW', 'GREEN'
                ]
            },
            {
                id: 'MOTORTYPE_GROUP',
                name: 'Motor type',
                description: 'the motor of you car',
                optionIds: [
                    'DIESEL', 'PETROL'
                ]
            },
            {
                id: 'MOTOR_GROUP',
                name: 'Motor',
                description: 'how powerful',
                optionIds: [
                    'D150', 'D250', 'P220', 'P450'
                ]
            },
            {
                id: 'PANORAMA_GROUP',
                name: 'Panoramic Roof',
                description: 'a glass roof for an open feeling',
                optionIds: [
                    'PANORAMAROOF'
                ]
            },
            {
                id: 'PANORAMATYPE_GROUP',
                name: 'Panoramic Roof type',
                description: 'size of your panorama roof',
                optionIds: [
                    'PANORAMASMALL', 'PANORAMALARGE'
                ]
            }
        ],
        rules: {
            basePrice: 10000,
            // defaultOptions: [],
            defaultOptions: ['BLUE', 'DIESEL', 'D150'],
            replacementGroups: {
                COLOR_GROUP: [
                    'BLUE', 'YELLOW', 'GREEN'
                ],
                MOTOR_GROUP: [
                    'DIESEL', 'PETROL'
                ],
                MOTORTYPE_GROUP: [
                    'D150', 'D250', 'P220', 'P450'
                ],
                PANORAMATYPE_GROUP: [
                    'PANORAMASMALL', 'PANORAMALARGE'
                ]
            },
            requirements: {
                D150: ['DIESEL'],
                D250: ['DIESEL'],
                P220: ['PETROL'],
                P450: ['PETROL']
            },
            incompatibilites: {
                PANORAMAROOF: ['PETROL']
            },
            priceList: {
                D150: 8000,
                D250: 11000,
                P220: 9000,
                P450: 16000,
                YELLOW: 200,
                GREEN: 500,
                PANORAMAROOF: 2000,
                PANORAMALARGE: 500
            }
        }
    }
]