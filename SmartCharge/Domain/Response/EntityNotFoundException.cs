using System;

namespace SmartCharge.Domain.Response;

public class EntityNotFoundException(string message) : Exception(message);