﻿using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Nethereum.Contracts
{
    public static class FunctionOuputDTOExtensions
    {
        private static readonly FunctionCallDecoder _functionCallDecoder = new FunctionCallDecoder();

        public static TFunctionOutputDTO DecodeOutput<TFunctionOutputDTO>(this TFunctionOutputDTO functionOuputDTO, string output) where TFunctionOutputDTO: IFunctionOutputDTO {
            return _functionCallDecoder.DecodeFunctionOutput(functionOuputDTO, output);
        }
    }
}