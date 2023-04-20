import type { Config } from '@jest/types';

const config: Config.InitialOptions = {
    preset: 'ts-jest',
    testEnvironment: 'jest-environment-jsdom',
    transform: {
        '^.+\\.(js|jsx|ts|tsx)$': 'babel-jest',
    },
    verbose: true,
    moduleNameMapper: {
        '\\.module\\.css$': 'identity-obj-proxy',
    },
};

export default config;