# MacAlgorithm Class

Represents a message authentication code (MAC) algorithm.

    public abstract class MacAlgorithm : Algorithm


## Inheritance Hierarchy

* [[Algorithm|Algorithm Class]]
    * **MacAlgorithm**
        * Blake2bMac
        * HmacSha256
        * HmacSha512


## [TOC] Summary


## Properties


### DefaultKeySize

Gets the default key size, in bytes.

    public int DefaultKeySize { get; }

#### Property Value

The default key size, in bytes.


### DefaultMacSize

Gets the default MAC size, in bytes.

    public int DefaultMacSize { get; }

#### Property Value

The default MAC size, in bytes.


### MaxKeySize

Gets the maximum key size, in bytes.

    public int MaxKeySize { get; }

#### Property Value

The maximum key size, in bytes.


### MaxMacSize

Gets the maximum MAC size, in bytes.

    public int MaxMacSize { get; }

#### Property Value

The maximum MAC size, in bytes.


### MinKeySize

Gets the minimum key size, in bytes.

    public int MinKeySize { get; }

#### Property Value

The minimum key size, in bytes.


### MinMacSize

Gets the minimum MAC size, in bytes.

    public int MinMacSize { get; }

#### Property Value

The minimum MAC size, in bytes.


## Methods


### Mac(Key, ReadOnlySpan<byte>)

Computes a message authentication code for the specified input data using the
specified key and returns it as an array of bytes.

    public byte[] Mac(
        Key key,
        ReadOnlySpan<byte> data)

#### Parameters

key
: The key to use for computing the message authentication code.

data
: The data to be authenticated.

#### Return Value

The computed message authentication code.

#### Exceptions

ArgumentNullException
: `key` is `null`.

ArgumentException
: `key.Algorithm` is not the same object as the current
    [[MacAlgorithm|MacAlgorithm Class]] object.

ObjectDisposedException
: `key` has been disposed.


### Mac(Key, ReadOnlySpan<byte>, int)

Computes a message authentication code for the specified input data using the
specified key and returns it as an array of bytes of the specified size.

    public byte[] Mac(
        Key key,
        ReadOnlySpan<byte> data,
        int macSize)

#### Parameters

key
: The key to use for computing the message authentication code.

data
: The data to be authenticated.

macSize
: The size, in bytes, of the message authentication code to compute.

#### Return Value

The computed message authentication code.

#### Exceptions

ArgumentNullException
: `key` is `null`.

ArgumentException
: `key.Algorithm` is not the same object as the current
    [[MacAlgorithm|MacAlgorithm Class]] object.

ArgumentOutOfRangeException
: `macSize` is less than
    [[MinMacSize|MacAlgorithm Class#MinMacSize]] or greater than
    [[MaxMacSize|MacAlgorithm Class#MaxMacSize]].

ObjectDisposedException
: `key` has been disposed.


### Mac(Key, ReadOnlySpan<byte>, Span<byte>)

Fills the specified span of bytes with a message authentication code for the
specified input data using the specified key.

    public void Mac(
        Key key,
        ReadOnlySpan<byte> data,
        Span<byte> mac)

#### Parameters

key
: The key to use for computing the message authentication code.

data
: The data to be authenticated.

mac
: The span to fill with the computed message authentication code.

#### Exceptions

ArgumentNullException
: `key` is `null`.

ArgumentException
: `key.Algorithm` is not the same object as the current
    [[MacAlgorithm|MacAlgorithm Class]] object.

ArgumentException
: `mac.Length` is less than
    [[MinMacSize|MacAlgorithm Class#MinMacSize]] or greater than
    [[MaxMacSize|MacAlgorithm Class#MaxMacSize]].

ObjectDisposedException
: `key` has been disposed.


### TryVerify(Key, ReadOnlySpan<byte>, ReadOnlySpan<byte>)

Attempts to verify the message authentication for the specified input data using
the specified key.

    public bool TryVerify(
        Key key,
        ReadOnlySpan<byte> data,
        ReadOnlySpan<byte> mac)

#### Parameters

key
: The key to use for verification.

data
: The data to be verified.

mac
: The message authentication code to be verified.

#### Return Value

`true` if verification succeeds; otherwise, `false`.

#### Exceptions

ArgumentNullException
: `key` is `null`.

ArgumentException
: `key.Algorithm` is not the same object as the current
    [[MacAlgorithm|MacAlgorithm Class]] object.

ObjectDisposedException
: `key` has been disposed.


### Verify(Key, ReadOnlySpan<byte>, ReadOnlySpan<byte>)

Verifies the message authentication code for the specified input data using the
specified key.

    public void Verify(
        Key key,
        ReadOnlySpan<byte> data,
        ReadOnlySpan<byte> mac)

#### Parameters

key
: The key to use for verification.

data
: The data to be verified.

mac
: The message authentication code to be verified.

#### Exceptions

ArgumentNullException
: `key` is `null`.

ArgumentException
: `key.Algorithm` is not the same object as the current
    [[MacAlgorithm|MacAlgorithm Class]] object.

ArgumentException
: `mac.Length` is less than
    [[MinMacSize|MacAlgorithm Class#MinMacSize]] or greater than
    [[MaxMacSize|MacAlgorithm Class#MaxMacSize]].

CryptographicException
: Verification failed.

ObjectDisposedException
: `key` has been disposed.


## Thread Safety

All members of this type are thread safe.


## Purity

All methods yield the same result for the same arguments.


## See Also

* API Reference
    * [[Algorithm Class]]
    * [[Key Class]]
