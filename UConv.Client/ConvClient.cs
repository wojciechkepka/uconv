using System;
using System.Net.Sockets;
using UConv.Core;

namespace UConv.Client
{
    class ConvClient : UTcpClient
    {
        public ConvClient(
            string hostname,
            int port
        ) : base(hostname, port)
        {
        }

        public Response ConvertRequest(string converter, string inputUnit, string outputUnit, string value)
        {
            Connect();
            using (NetworkStream ns = GetStream())
            {
                var resp = writeRequest<ConvRequest>(ns, "/convert", new ConvRequest(converter, inputUnit, outputUnit, value));
                try
                {
                    return (ConvResponse)Response.FromData<ConvResponse>(resp);
                }
                catch (Exception _)
                {
                    try
                    {
                        return (ErrResponse)Response.FromData<ErrResponse>(resp);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidResponse(ex.Message, resp);
                    }
                }
            }
        }

        public Response ConverterListRequest()
        {
            Connect();
            using (NetworkStream ns = GetStream())
            {
                var resp = writeRequest<ConvListRequest>(ns, "/converters", new ConvListRequest());
                try
                {
                    return (ConvListResponse)Response.FromData<ConvListResponse>(resp);
                }
                catch (Exception _)
                {
                    try
                    {
                        return (ErrResponse)Response.FromData<ErrResponse>(resp);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidResponse(ex.Message, resp);
                    }
                }
            }
        }

        public Response RateMeRequest(string hostname, int rating)
        {
            Connect();
            using (NetworkStream ns = GetStream())
            {
                var resp = writeRequest<RateMeRequest>(ns, "/rateme", new RateMeRequest(hostname, rating));
                try
                {
                    return (RateMeResponse)Response.FromData<RateMeResponse>(resp);
                }
                catch (Exception _)
                {
                    try
                    {
                        return (ErrResponse)Response.FromData<ErrResponse>(resp);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidResponse(ex.Message, resp);
                    }
                }
            }
        }
    }
}
