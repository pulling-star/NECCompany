# Key Class

Represents a symmetric key or an asymmetric key pair.

    public sealed class Key : IDisposable


## [TOC] Summary


## Constructors


### Key(Algorithm, KeyFlags)

Initializes a new instance of the [[Key|Key Class]] class with a random key.

    public Key(
        Algorithm algorithm,
        KeyFlags flags = KeyFlags.None)

#### Parameters

algorithm
: The algorithm for the key.

flags
: A bitwise combination of [[KeyFlags|KeyFlags Enum]] values that specifies
    the flags for the new key.

#### Exceptions

ArgumentNullException
: `algorithm` is null.

NotSupportedException
: The specified algorithm does not support keys.


## Properties


### Algorithm

Gets the algorithm for the key.

    public Algorithm Algorithm { get; }

#### Property value

An instance of the [[Algorithm|Algorithm Class]] class.


### Flags

Gets the flags for the key.

    public KeyFlags Flags { get; }

#### Property value

A bitwise combination of [[KeyFlags|KeyFlags Enum]] values that specifies the
flags for the key.


### PublicKey

Gets the public key if the current instance of the [[Key|Key Class]] class
represents a key pair.

    public PublicKey PublicKey { get; }

#### Property value

An instance of the [[PublicKey|PublicKey Class]] class if the current instance
of the [[Key|Key Class]] class represents a key pair; otherwise, `null`.


## Methods


### Create(Algorithm, KeyFlags)

Creates a new instance of the [[Key|Key Class]] class with a random key.

    public static Key Create(
        Algorithm algorithm,
        KeyFlags flags = KeyFlags.None)

#### Parameters

algorithm
: The algorithm for the key.

flags
: A bitwise combination of [[KeyFlags|KeyFlags Enum]] values that specifies
    the flags for the new key.

#### Exceptions

ArgumentNullException
: `algorithm` is null.

NotSupportedException
: The specified algorithm does not support keys.


### Import(Algorithm, ReadOnlySpan<byte>, KeyBlobFormat, KeyFlags)

Imports the specified key BLOB in the specified format.

    public static Key Import(
        Algorithm algorithm,
        ReadOnlySpan<byte> blob,
        KeyBlobFormat format,
        KeyFlags flags = KeyFlags.None)

#### Parameters

algorithm
: The algorithm for the imported key.

blob
: The key BLOB to import.

format
: One of the [[KeyBlobFormat|KeyBlobFormat Enum]] values that specifies the
    format of the key BLOB.

flags
: A bitwise combination of [[KeyFlags|KeyFlags Enum]] values that specifies
    the flags for the imported key.

#### Return value

A new instance of the [[Key|Key Class]] class that represents the imported key.

#### Exceptions

ArgumentNullException
: `algorithm` is null.

FormatException
: The key BLOB is not in the correct format or the format is not supported.

NotSupportedException
: The specified algorithm does not support importing keys.


### TryImport(Algorithm, ReadOnlySpan<byte>, KeyBlobFormat, KeyFlags, Key)

Attempts to import the specified key BLOB in the specified format.

    public static bool TryImport(
        Algorithm algorithm,
        ReadOnlySpan<byte> blob,
        KeyBlobFormat format,
        KeyFlags flags,
        out Key result)

#### Parameters

algorithm
: The algorithm for the imported key.

blob
: The key BLOB to import.

format
: One of the [[KeyBlobFormat|KeyBlobFormat Enum]] values that specifies the
    format of the key BLOB.

flags
: A bitwise combination of [[KeyFlags|KeyFlags Enum]] values that specifies
    the flags for the imported key.

result
: When this method returns, contains a new instance of the [[Key|Key Class]]
    class that represents the imported key, or `null` if the import failed.

#### Return value

`true` if the key BLOB was imported; otherwise, `false`.

#### Exceptions

ArgumentNullException
: `algorithm` is null.

NotSupportedException
: The specified algorithm does not support importing keys.


### Dispose()

Securely erases the key and releases all resources used by the current instance
of the [[Key|Key Class]] class.

    public void Dispose();


### Export(KeyBlobFormat)

Exports the key into a BLOB, in the specified format.

    public byte[] Export(
        KeyBlobFormat format)


#### Parameters

format
: One of the [[KeyBlobFormat|KeyBlobFormat Enum]] values that specifies the
    format of the key BLOB.

#### Returns

A BLOB that contains the key in the specified format.

#### Exceptions

FormatException
: The algorithm for the key does not support the specified format.

InvalidOperationException
: The flags for the key do not allow the key to be exported.

NotSupportedException
: The algorithm for the key does not support exporting keys.

ObjectDisposedException
: The key has been disposed.


## See also

* API Reference
    * [[Algorithm Class]]
    * [[KeyBlobFormat Enum]]
    * [[KeyFlags Enum]]
    * [[PublicKey Class]]
