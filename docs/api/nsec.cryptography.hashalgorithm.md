# HashAlgorithm Class

Represents a cryptographic hash algorithm.

    public abstract class HashAlgorithm : Algorithm


## Inheritance Hierarchy

* [[Algorithm|Algorithm Class]]
    * **HashAlgorithm**
        * Blake2b
        * Sha256
        * Sha512


## [TOC] Summary


## Properties


### DefaultHashSize

Gets the default hash size, in bytes.

    public int DefaultHashSize { get; }

#### Property Value

The default hash size, in bytes.



### MaxHashSize

Gets the maximum hash size, in bytes.

    public int MaxHashSize { get; }

#### Property Value

The maximum hash size, in bytes.


### MinHashSize

Gets the minimum hash size, in bytes.

    public int MinHashSize { get; }

#### Property Value

The minimum hash size, in bytes.


## Methods


### Hash(ReadOnlySpan<byte>)

Computes a hash for the specified input data and returns it as an array of
bytes.

    public byte[] Hash(
        ReadOnlySpan<byte> data)

#### Parameters

data
: The input data to compute the hash for.

#### Return Value

The computed hash.

### Hash(ReadOnlySpan<byte>, int)

Computes a hash for the specified input data and returns it as an array of
bytes of the specified size.

    public byte[] Hash(
        ReadOnlySpan<byte> data,
        int hashSize)

#### Parameters

data
: The input data to compute the hash for.

hashSize
: The size, in bytes, of the hash to compute.

#### Return Value

The computed hash.

#### Exceptions

ArgumentOutOfRangeException
: `hashSize` is less than [[MinHashSize|HashAlgorithm Class#MinHashSize]] or
    greater than [[MaxHashSize|HashAlgorithm Class#MaxHashSize]].


### Hash(ReadOnlySpan<byte>, Span<byte>)

Fills the specified span of bytes with a hash for the specified input data.

    public void Hash(
        ReadOnlySpan<byte> data,
        Span<byte> hash)

#### Parameters

data
: The input data to compute the hash for.

hash
: The span to fill with the computed hash.

#### Exceptions

ArgumentException
: `hash.Length` is less than
    [[MinHashSize|HashAlgorithm Class#MinHashSize]] or greater than
    [[MaxHashSize|HashAlgorithm Class#MaxHashSize]].


### TryVerify(ReadOnlySpan<byte>, ReadOnlySpan<byte>)

Attempts to verify the hash for the specified input data.

    public bool TryVerify(
        ReadOnlySpan<byte> data,
        ReadOnlySpan<byte> hash)

#### Parameters

data
: The data to be verified.

hash
: The hash to be verified.

#### Return Value

`true` if verification succeeds; otherwise, `false`.


### Verify(ReadOnlySpan<byte>, ReadOnlySpan<byte>)

Verifies the hash for the specified input data.

    public void Verify(
        ReadOnlySpan<byte> data,
        ReadOnlySpan<byte> hash)

#### Parameters

data
: The data to be verified.

hash
: The hash to be verified.

#### Exceptions

CryptographicException
: Verification failed.


## Thread Safety

All members of this type are thread safe.


## Purity

All methods yield the same result for the same arguments.


## See Also

* API Reference
    * [[Algorithm Class]]
