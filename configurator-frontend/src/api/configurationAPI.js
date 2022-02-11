import axios from 'axios'
import { baseURL, LOCAL_DATA } from './general'

export const fetchId = (productId) => {
    if (LOCAL_DATA) {
        return fetchApiTest(productId)
    }

    return new Promise((resolve, reject) => {
        axios.get(`${baseURL}/configuration/${productId}`)
        
        // axios.get(`${baseURL}/configuration/Golf`)
        .then(res => {
            resolve(res.data)
        })
        .catch(err => {
            reject(err.toString())
        })
    })
}

export const postConfiguration = (newConfiguration) => {
    return new Promise((resolve, reject) => {
        if (LOCAL_DATA) {
            reject('Posting a new configuration not available in test mode!')
            return
        }

        const data = {
            ...newConfiguration
        }
        console.log(data)
        axios.post(`${baseURL}/configuration`, data)
        .then(res => {
            resolve(res.data)
        })
        .catch(err => {
            console.log(err)
            reject('Api unreachable')
        })
    })
}

// A mock api request function to mimic making an async request for data
const testDelay = 0;
function fetchApiTest(configId) {
    return new Promise((resolve, reject) =>
        setTimeout(() => {

            const conf = configurations.find(c => c.configId === configId)
            if (!conf) {
                reject('NO_CONFIGURATION_FOUND')
                return
            }
            else if (!conf.options || !conf.optionGroups || !conf.optionSections || !conf.rules) {
                reject('CONFIGURATION_INVALID')
                return
            }

            resolve(conf)

        }, testDelay)
    )
}

const configurations = [
    {
        configId: '0',
        name: 'Car',
        description: 'automobile',
        images: [], // 'vw-golf-r-2021.jpg', 'vw-golf-r-2021.jpg', 'vw-golf-r-2021.jpg'
        options: [
            {
                id: 'BLUE',
                name: 'Blue',
                description: 'A blue color',
            },
            {
                id: 'YELLOW',
                name: 'Yellow',
                description: 'A yellow color',
            },
            {
                id: 'GREEN',
                name: 'Green',
                description: 'A green color',
            },
            {
                id: 'DIESEL',
                name: 'Diesel Motor',
                description: 'a motor driven by diesel fuel',
            },
            {
                id: 'PETROL',
                name: 'Petrol Motor',
                description: 'a motor driven by petrol fuel',
            },
            {
                id: 'D150',
                name: '150 PS Diesel Motor',
                description: 'a diesel V4 motor with 150 PS',
            },
            {
                id: 'D250',
                name: '250 PS Diesel Motor',
                description: 'a diesel V6 motor with 250 PS',
            },
            {
                id: 'P220',
                name: '220 PS Petrol Motor',
                description: 'a petrol V6 motor with 220 PS',
            },
            {
                id: 'P450',
                name: '450 PS Petrol Motor',
                description: 'a petrol V8 motor with 450 PS',
            },
            {
                id: 'PANORAMAROOF',
                name: 'Panoramic Roof',
                description: 'a glass roof for an open feeling',
            },
            {
                id: 'PANORAMASMALL',
                name: 'Small Panorama',
                description: 'a small glass roof',
            },
            {
                id: 'PANORAMALARGE',
                name: 'Large Panorama',
                description: 'a large glass roof for an amazing open feeling',
            },
            {
                id: 'HEATED_SEATS',
                name: 'Heated Seats',
                description: 'the two seats in the front can be heated',
            }, 
            {
                id:  'HIGH_QUALITY_SOUND_SYSTEM',
                name: 'High Quality Sound System',
                description: 'premium sound system with high res audio',
            }, 
            {
                id: 'DRIVE_ASSISTENCE',
                name: 'Drive Assistence',
                description: 'extra driving assistence including cruise control, adaptive cruise control and a lane keeping assistent',
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
            },
            {
                id: 'EXTRAS',
                name: 'Extras',
                optionGroupIds: [
                    'EXTRAS_GROUP'
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
                ],
                required: true
            },
            {
                id: 'MOTORTYPE_GROUP',
                name: 'Motor type',
                description: 'type of your motor',
                optionIds: [
                    'DIESEL', 'PETROL'
                ],
                required: true
            },
            {
                id: 'MOTOR_GROUP',
                name: 'Motor',
                description: 'specific motor',
                optionIds: [
                    'D150', 'D250', 'P220', 'P450'
                ],
                required: true
            },
            {
                id: 'PANORAMA_GROUP',
                name: 'Panoramic Roof',
                description: 'a glass roof for an open feeling',
                optionIds: [
                    'PANORAMAROOF'
                ],
                required: false
            },
            {
                id: 'PANORAMATYPE_GROUP',
                name: 'Panoramic Roof type',
                description: 'size of your panorama roof',
                optionIds: [
                    'PANORAMASMALL', 'PANORAMALARGE'
                ],
                required: true
            },
            {
                id: 'EXTRAS_GROUP',
                name: 'Extras',
                description: 'Additional Features For Your Car',
                optionIds: [
                    'HEATED_SEATS', 'HIGH_QUALITY_SOUND_SYSTEM', 'DRIVE_ASSISTENCE'
                ],
                required: false
            }
        ],
        rules: {
            basePrice: 10000,
            // defaultOptions: [],
            // defaultOptions: ['BLUE', 'DIESEL', 'D150'],

            defaultModel: 'Golf',
            models: [
                {
                    name: 'Golf',
                    options: ['BLUE', 'DIESEL', 'D150', 'DRIVE_ASSISTENCE'],
                    description: "base model of this car"
                },
                {
                    name: 'Golf GTI',
                    options: ['YELLOW', 'PETROL', 'P220', 'HEATED_SEATS', 'HIGH_QUALITY_SOUND_SYSTEM'],
                    description: "advanced model"
                }
            ],
            replacementGroups: {
                COLOR_GROUP: [
                    'BLUE', 'YELLOW', 'GREEN'
                ],
                MOTORTYPE_GROUP: [
                    'DIESEL', 'PETROL'
                ],
                MOTOR_GROUP: [
                    'D150', 'D250', 'P220', 'P450'
                ],
                PANORAMATYPE_GROUP: [
                    'PANORAMASMALL', 'PANORAMALARGE'
                ]
            },
            groupRequirements: {
                PANORAMATYPE_GROUP: ['PANORAMA_GROUP'],
                MOTOR_GROUP: ['MOTORTYPE_GROUP']
            },
            requirements: {
                D150: ['DIESEL'],
                D250: ['DIESEL'],
                P220: ['PETROL'],
                P450: ['PETROL'],
                PANORAMASMALL: ['PANORAMAROOF'],
                PANORAMALARGE: ['PANORAMAROOF']
            },
            incompatibilities: {
                PANORAMAROOF: ['PETROL'],
                PANORAMASMALL: ['BLUE']
            },
            priceList: {
                D150: 8000,
                D250: 11000,
                P220: 9000,
                P450: 16000,
                YELLOW: 200,
                GREEN: 500,
                PANORAMAROOF: 2000,
                PANORAMALARGE: 500,
                HEATED_SEATS: 500, 
                HIGH_QUALITY_SOUND_SYSTEM: 250,
                DRIVE_ASSISTENCE: 1500
            }
        }
    },
    {
        configId: '1',
        images: [],
        options: [
            {
                id: 'ALLOY19',
                name: 'ALLOY19',
                description: 'ALLOY19',
                groupId: 'WHEELS'
            },
            {
                id: 'STEEL16',
                name: 'STEEL16',
                description: 'STEEL16',
                groupId: 'WHEELS'
            },
            {
                id: 'RED',
                name: 'RED',
                description: 'RED',
                groupId: 'COLOR_GROUP'
            },
            {
                id: 'BLUE',
                name: 'BLUE',
                description: 'RED',
                groupId: 'COLOR_GROUP'
            }
        ],
        optionSections: [
            {
                id: 'EXTERIOR',
                optionGroupIds: [
                    'WHEELS', 'COLOR_GROUP'
                ]
            }
        ],
        optionGroups: [
            {
                id: 'WHEELS',
                name: 'WHEELS',
                description: 'WHEELS',
                optionIds: [
                    'ALLOY19', 'STEEL16'
                ],
                required: true,
                replacement: true
            },
            {
                id: 'COLOR_GROUP',
                name: 'COLOR_GROUP',
                description: 'Desc',
                optionIds: [
                    'BLUE', 'RED'
                ],
                required: false,
                replacement: false
            }
        ],
        rules: {
            basePrice: 500,
            // defaultOptions: [/*BLUE*/],
            defaultModel: '',
            models: [
                {
                    id: 'Sport',
                    name: 'Sport',
                    description: 'Desc',
                    options: ['ALLOY19', 'RED'],
                },
                {
                    id: 'Basic',
                    name: 'Basic',
                    description: 'Desc',
                    options: ['STEEL16', 'BLUE'],
                }
            ],
            // replacementGroups: {
            //     // COLOR_GROUP: [
            //     //     'BLUE'
            //     // ]
            // },
            groupRequirements: {
                // PANORAMATYPE_GROUP: ['PANORAMA_GROUP']
                COLOR_GROUP: ['WHEELS'],
                WHEELS: ['COLOR_GROUP']
            },
            requirements: {
                BLUE: ['STEEL16'],
                ALLOY19: ['RED']
            },
            incompatibilities: {
                // PANORAMAROOF: ['PETROL']
                BLUE: ['ALLOY19'],
                STEEL16: ['RED']
            },
            priceList: {
                'BLUE': 200
            }
        }
    }
]