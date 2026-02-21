# lpath = lib.makeLibraryPath [ stdenv.cc.cc libunwind libuuid icu openssl zlib curl ];
export LD_LIBRARY_PATH=$PWD/bin/Debug/net10.0/runtimes/linux-x64/native:$LD_LIBRARY_PATH